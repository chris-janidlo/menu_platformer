using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

[RequireComponent(typeof(Rigidbody2D), typeof(ColoredHealth))]
public class Mage : MonoBehaviour
{
    public const float MaxMana = 100;

    [SerializeField]
    float _mana = MaxMana;
    public float Mana
    {
        get => _mana;
        set => _mana = Mathf.Clamp(value, 0, MaxMana);
    }

    public bool FacingLeft => Wand.flipX;
    public bool Active => MageSquad.Instance.ActiveMage == this;

    [Header("Stats")]
    public MagicColor Color;
    public float NormalManaRecovery, PotionManaRecovery, ManaPotActiveTime;
    public float HealthPotHeal;
    public float BurstCost, LineCost, LobCost;
    public float Ability1CooldownTime, Ability2CooldownTime;

    public float MoveSpeed;
    public float GroundAcceleration;
    public float AirAcceleration;
    public float JumpSpeedBurst, JumpSpeedCut;
    public Vector2 HighJumpBurst, LongJumpBurst;
    public float Gravity;
    public float JumpFudgeTime;
    public float GroundedFudgeVertical = 0.1f, GroundedFudgeHorizontal = 0.1f;
    public float ParticleStopTime;

    [Header("Ability Stats")]
    public float NimbilityTime;
    public float NimbilityNewSpeed, NimbilityNewJumpBurst;
    public float BombashDamage;
    public float BombashShakeTime, BombashShakeAmount;
    public float RejuveHeal;
    public float TimeSlowAmount;
    public float TimeSlowTime;

    [Header("References")]
    public ColoredHealth Health;
    public ColorMapApplier Body;
    public SpriteRenderer Wand;
    public ColorMapApplierParticles Particles;
    public BurstBullet BurstPrefab;
    public LineBullet LinePrefab;
    public LobBullet LobPrefab;

    public float Ability1Cooldown { get; private set; }
    public float Ability2Cooldown { get; private set; }

    Rigidbody2D rb;
    float halfHeight;
    // while we're long or high jumping, we don't want the player to be able to give input, nor do we want the regular air speed constraints to apply, so we have a special platforming state to compensate. we separate the part of the special jump that happens immediately at launch and the part that happens throughout the air to work around grounded checks; if we only keep one variable, it leads to a race condition with the isGrounded() call in platform(), since we need to set the fact that we're done special jumping on landing but don't want that to happen while we're still launching
    bool specialJumpLaunch, specialJumpAir;
    Vector2 groundedExtents;
    float timeSinceLastJumpPress = float.MaxValue;

    float manaPotTimer;

    bool ability1Flag, ability2Flag;

    bool gameStarted;
    float moveInput;

    bool specialJumping => specialJumpLaunch || specialJumpAir;

    // grounded needs to call the Physics2D.BoxCast that returns an array of results in order to ignore ourselves without ignoring other player objects. that method takes its options as a ContactFilter2D, but that never changes between calls to isGrounded, so we create it once for free GC savings
    ContactFilter2D groundedFilter;

    SpriteRenderer bodyRenderer;

    IEnumerator particleEnum;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false; // so we can follow the selection cursor

        var extents = GetComponent<Collider2D>().bounds.extents;
        halfHeight = extents.y;
        groundedExtents = new Vector2
        (
            2 * extents.x + GroundedFudgeHorizontal,
            GroundedFudgeVertical
        );

        groundedFilter = new ContactFilter2D();
        // bullets don't affect grounded state
        groundedFilter.layerMask = ~LayerMask.NameToLayer("Player Bullet");

        Body.ChangeColor(Color);
        Particles.ChangeColor(Color);
        Health.Color = Color;
        Health.Death.AddListener(die);

        bodyRenderer = Body.GetComponent<SpriteRenderer>();
    }

    void Update ()
    {
        if (!gameStarted) return;

        bodyRenderer.SetAlpha(Health.Dead ? .5f : 1);

        platform();

        Mana += (manaPotTimer <= 0 ? NormalManaRecovery : PotionManaRecovery) * Time.deltaTime;
        manaPotTimer = Mathf.Max(manaPotTimer - Time.deltaTime, 0);

        Ability1Cooldown -= Time.deltaTime;
        Ability2Cooldown -= Time.deltaTime;

        if (moveInput < 0 && !Wand.flipX)
        {
            Wand.flipX = true;
        }

        if (moveInput > 0 && Wand.flipX)
        {
            Wand.flipX = false;
        }
    }

    public void StartGame ()
    {
        gameStarted = true;
        rb.simulated = true;
    }

    public void CastBurst ()
    {
        var cost = BurstCost;
        
        if (Mana < cost)
        {
            CantDoThatFeedback.Instance.DisplayMessage("not enough mana!");
            return;
        }
        Mana -= cost;

        var dirs = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(1, -1),
            new Vector2(0, -1),
            new Vector2(-1, -1),
            new Vector2(-1, 0),
            new Vector2(-1, 1),
        };

        foreach (var dir in dirs)
        {
            var bullet = Instantiate(BurstPrefab, transform.position, Quaternion.identity);
            bullet.Initialize(dir, Color);
        }
    }

    public void CastLine ()
    {
        var cost = LineCost;
        
        if (Mana < cost)
        {
            CantDoThatFeedback.Instance.DisplayMessage("not enough mana!");
            return;
        }
        Mana -= cost;

        var bullet = Instantiate(LinePrefab, transform.position, Quaternion.identity);
        bullet.Initialize(FacingLeft, Color);
    }

    public void CastLob ()
    {
        var cost = LobCost;
        
        if (Mana < cost)
        {
            CantDoThatFeedback.Instance.DisplayMessage("not enough mana!");
            return;
        }
        Mana -= cost;

        var bullet = Instantiate(LobPrefab, transform.position, Quaternion.identity);
        bullet.Initialize(FacingLeft, Color);
    }

    public void LongJump ()
    {
        if (isGrounded())
        {
            StartCoroutine(setSpecialJump());
            rb.velocity = new Vector2
            (
                LongJumpBurst.x * (FacingLeft ? -1 : 1),
                LongJumpBurst.y
            );
        }
        else
        {
            CantDoThatFeedback.Instance.DisplayMessage("can't jump in air!");
        }
    }

    public void HighJump ()
    {
        if (isGrounded())
        { 
            StartCoroutine(setSpecialJump());
            rb.velocity = new Vector2
            (
                HighJumpBurst.x * (FacingLeft ? -1 : 1),
                HighJumpBurst.y
            );
        }
        else
        {
            CantDoThatFeedback.Instance.DisplayMessage("can't jump in air!");            
        }
    }

    // delay setting specialJumpAir to true until we're no longer grounded
    IEnumerator setSpecialJump ()
    {
        specialJumpLaunch = true;

        yield return new WaitUntil(() => !isGrounded());

        specialJumpLaunch = false;
        specialJumpAir = true;
    }

    void playParticles ()
    {
        if (particleEnum != null) StopCoroutine(particleEnum);
        StartCoroutine(particleEnum = particleRoutine());
    }

    IEnumerator particleRoutine ()
    {
        Particles.ParticleSystem.Play();

        yield return new WaitForSeconds(ParticleStopTime);

        Particles.ParticleSystem.Stop();
    }

    public void Special1 ()
    {
        if (Ability1Cooldown > 0)
        {
            CantDoThatFeedback.Instance.DisplayMessage($"wait {Mathf.RoundToInt(Ability1Cooldown)} more seconds!");
            return;
        }

        bool castSuccessful = false;
        
        switch (Color)
        {
            case MagicColor.Red:
                castSuccessful = nimbility();
                break;

            case MagicColor.Green:
                castSuccessful = rejuve();
                break;

            case MagicColor.Blue:
                castSuccessful = swap();
                break;
        }

        if (castSuccessful) Ability1Cooldown = Ability1CooldownTime;
    }

    bool nimbility ()
    {
        if (ability1Flag)
        {
            CantDoThatFeedback.Instance.DisplayMessage("ability already active!");
            return false;
        }

        playParticles();
        StartCoroutine(nimbilityRoutine());
        return true;
    }

    IEnumerator nimbilityRoutine ()
    {
        ability1Flag = true;

        var speedMem = MoveSpeed;
        var jumpMem = JumpSpeedBurst;

        MoveSpeed = NimbilityNewSpeed;
        JumpSpeedBurst = NimbilityNewJumpBurst;

        yield return new WaitForSeconds(NimbilityTime);

        MoveSpeed = speedMem;
        JumpSpeedBurst = jumpMem;

        ability1Flag = false;
    }

    bool rejuve ()
    {
        var lowestMage = MageSquad.Instance.RedMage.Health.CurrentHealth < MageSquad.Instance.BlueMage.Health.CurrentHealth ? MageSquad.Instance.RedMage : MageSquad.Instance.BlueMage;

        if (lowestMage.Health.CurrentHealth == 100)
        {
            CantDoThatFeedback.Instance.DisplayMessage("other mages already full health!");
            return false;
        }

        lowestMage.Health.Heal(RejuveHeal);
        lowestMage.playParticles();
        return true;
    }

    bool swap ()
    {
        // want to find the derangement https://en.wikipedia.org/wiki/Derangement since that will feel the best
        // there are only 2 derangements of a set of 3 elements https://math.stackexchange.com/a/2718953/574705

        List<List<Vector3>> derangements = new List<List<Vector3>>
        {
            new List<Vector3>
            {
                MageSquad.Instance.GreenMage.transform.position,
                MageSquad.Instance.BlueMage.transform.position,
                MageSquad.Instance.RedMage.transform.position
            },
            new List<Vector3>
            {
                MageSquad.Instance.BlueMage.transform.position,
                MageSquad.Instance.RedMage.transform.position,
                MageSquad.Instance.GreenMage.transform.position
            },
        };

        List<Vector3> derangement = derangements[UnityEngine.Random.Range(0, 1)];

        for (int i = 0; i < 3; i++)
        {
            var mage = MageSquad.Instance[(MagicColor) i];
            mage.transform.position = derangement[i];
            mage.playParticles();
        }

        return true;
    }

    public void Special2 ()
    {
        if (Ability2Cooldown > 0)
        {
            CantDoThatFeedback.Instance.DisplayMessage($"wait {Mathf.RoundToInt(Ability2Cooldown)} more seconds!");
            return;
        }

        bool castSuccessful = false;

        switch (Color)
        {
            case MagicColor.Red:
                castSuccessful = bombash();
                break;

            case MagicColor.Green:
                castSuccessful = recoup();
                break;

            case MagicColor.Blue:
                castSuccessful = timeStop();
                break;
        }

        if (castSuccessful) Ability2Cooldown = Ability2CooldownTime;
    }

    bool bombash ()
    {
        var enemies = FindObjectsOfType<BaseEnemy>();

        if (enemies.Length == 0)
        {
            CantDoThatFeedback.Instance.DisplayMessage("no enemies to hurt!");
            return false;
        }

        foreach (var enemy in enemies)
        {
            enemy.Health.PureDamage(BombashDamage);
        }

        CameraCache.Main.ShakeScreen2D(BombashShakeTime, BombashShakeAmount);

        return true;
    }

    bool recoup ()
    {
        Mage toHeal;

        if (MageSquad.Instance.RedMage.Health.Dead)
        {
            toHeal = MageSquad.Instance.RedMage;
        }
        else if (MageSquad.Instance.BlueMage.Health.Dead)
        {
            toHeal = MageSquad.Instance.BlueMage;
        }
        else
        {
            CantDoThatFeedback.Instance.DisplayMessage("no one is dead!");
            return false;
        }

        toHeal.Health.FullHeal();
        toHeal.playParticles();

        return true;
    }

    bool timeStop ()
    {
        if (ability2Flag)
        {
            CantDoThatFeedback.Instance.DisplayMessage("time is already slowed!");
            return false;
        }

        StartCoroutine(timeStopRoutine());
        return true;
    }

    IEnumerator timeStopRoutine ()
    {
        Time.timeScale = TimeSlowAmount;
        ability2Flag = true;
        
        yield return new WaitForSecondsRealtime(TimeSlowTime);

        Time.timeScale = 1;
        ability2Flag = false;
    }

    public void DrinkHealthPotion ()
    {
        if (MageSquad.Instance.HealthPots == 0)
        {
            CantDoThatFeedback.Instance.DisplayMessage("not enough band aids!");
            return;
        }

        MageSquad.Instance.HealthPots--;
        Health.Heal(HealthPotHeal);
    }

    public void DrinkManaPotion ()
    {
        if (MageSquad.Instance.ManaPots == 0)
        {
            CantDoThatFeedback.Instance.DisplayMessage("not enough mana potions!");
            return;
        }

        MageSquad.Instance.ManaPots--;
        manaPotTimer += ManaPotActiveTime;
    }

    void die ()
    {
        MagicColor? activeColor = null;
        foreach (var mage in MageSquad.Instance)
        {
            if (!mage.Health.Dead)
            {
                activeColor = mage.Color;
                break;
            }
        }

        if (activeColor == null)
        {
            EndScreen.GameOver.StartSequence();
        }
        else if (Active)
        {
            MageSquad.Instance.SetActive((MagicColor) activeColor);
        }
    }

    void platform ()
    {
        bool platformingActive = Active && !specialJumping;

        moveInput = platformingActive ? Input.GetAxisRaw("Move") : 0;
        bool jumpHold = platformingActive ? Input.GetButton("Jump") : false;
        if (platformingActive && Input.GetButtonDown("Jump"))
        {
            timeSinceLastJumpPress = 0;
        }
        
        if (isGrounded() && !specialJumpLaunch)
        {
            specialJumpAir = false;

            float newX = 0;

            if (moveInput != 0)
            {
                newX = rb.velocity.x + moveInput * GroundAcceleration * Time.deltaTime;
            }

            newX = Mathf.Clamp(newX, -MoveSpeed, MoveSpeed);

            rb.velocity = Vector2.right * newX;

            if (timeSinceLastJumpPress <= JumpFudgeTime)
            {
                rb.velocity = new Vector3(rb.velocity.x, JumpSpeedBurst);
            }
        }
        else
        {
            var newX = rb.velocity.x + moveInput * AirAcceleration * Time.deltaTime;
            if (!specialJumping) newX = Mathf.Clamp(newX, -MoveSpeed, MoveSpeed);

            var newY = rb.velocity.y - Gravity * Time.deltaTime;

            if (platformingActive && !jumpHold && newY > JumpSpeedCut)
            {
                newY = JumpSpeedCut;
            }

            rb.velocity = new Vector2(newX, newY);
        }

        timeSinceLastJumpPress += Time.deltaTime;
    }

    bool isGrounded ()
    {
        var groundedResults = new RaycastHit2D[2]; // don't need more than two; one for us, one for the ground
        Physics2D.BoxCast(transform.position, groundedExtents, 0, Vector2.down, groundedFilter, groundedResults, halfHeight);

        foreach (var result in groundedResults)
        {
            if (result.collider != null && result.collider.gameObject != gameObject)
            {
                return true;
            }
        }

        return false;
    }
}
