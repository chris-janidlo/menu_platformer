using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Mage : MonoBehaviour
{
    [Serializable]
    public class AbilityCosts
    {
        public float Special1, Special2;
    }

    public const float MaxHealth = 100, MaxMana = 100;

    [SerializeField]
    float _health = MaxHealth, _mana = MaxMana;

    public float Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
            
            if (_health == 0)
            {
                die();
            }
            else if (Dead)
            {
                revive();
            }
        }
    }

    public float Mana
    {
        get => _mana;
        set => _mana = Mathf.Clamp(value, 0, MaxMana);
    }

    public bool Dead { get; private set; }

    public bool FacingLeft => Wand.flipX;

    [Header("Stats")]
    public MagicColor Color;
    public float ManaGain;
    public SpellPowerContainer BurstCosts, LineCosts, LobCosts;
    public AbilityCosts RedAbilityCosts, GreenAbilityCosts, BlueAbilityCosts;

    public float MoveSpeed;
    public float GroundAcceleration;
    public float AirAcceleration;
    public float JumpSpeedBurst, JumpSpeedCut;
    public Vector2 HighJumpBurst, LongJumpBurst;
    public float Gravity;
    public float JumpFudgeTime;
    public float GroundedFudgeVertical = 0.1f, GroundedFudgeHorizontal = 0.1f;

    [Header("Ability Stats")]
    public float NimbilityTime;
    public float NimbilityNewSpeed, NimbilityNewJumpBurst;
    public float BombashDamage;
    public float RejuveHeal;
    public float RecoupHeal;
    public Collider2D EmbankPrefab;
    public float TimeSlowAmount;
    public float TimeSlowTime;

    [Header("References")]
    public ColorMapApplier Visuals;
    public SpriteRenderer Wand;
    public BurstBullet BurstPrefab;
    public LineBullet LinePrefab;
    public LobBullet LobPrefab;

    Rigidbody2D rb;
    float halfHeight;
    bool specialJumping;
    Vector2 groundedExtents;
    float timeSinceLastJumpPress = float.MaxValue;

    float speedMem, jumpMem, ability1Timer, ability2Timer;

    bool active, gameStarted;
    float moveInput;

    // grounded needs to call the Physics2D.BoxCast that returns an array of results in order to ignore ourselves without ignoring other player objects. that method takes its options as a ContactFilter2D, but that never changes between calls to isGrounded, so we create it once for free GC savings
    ContactFilter2D groundedFilter;

    void Awake ()
    {
        Visuals.Color = Color;
    }

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false; // so we can follow the selection cursor

        var extents = GetComponent<Collider2D>().bounds.extents;
        halfHeight = extents.y;
        groundedExtents = new Vector2
        (
            extents.x + GroundedFudgeHorizontal / 2,
            GroundedFudgeVertical / 2
        );

        groundedFilter = new ContactFilter2D();
        // bullets don't affect grounded state
        groundedFilter.layerMask = ~LayerMask.NameToLayer("Player Bullet");
    }

    void Update ()
    {
        if (!gameStarted) return;

        platform();

        ability1Timer -= Time.deltaTime;
        ability2Timer -= Time.deltaTime;

        Mana += ManaGain * Time.deltaTime;

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

    // returns whether the cast was successful
    public void CastBurst (SpellPower power)
    {
        var cost = BurstCosts[power];
        
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
            bullet.Initialize(dir, Color, power);
        }
    }

    // returns whether the cast was successful
    public void CastLine (SpellPower power)
    {
        var cost = LineCosts[power];
        
        if (Mana < cost)
        {
            CantDoThatFeedback.Instance.DisplayMessage("not enough mana!");
            return;
        }
        Mana -= cost;

        var bullet = Instantiate(LinePrefab, transform.position, Quaternion.identity);
        bullet.Initialize(FacingLeft, Color, power);
    }

    // returns whether the cast was successful
    public void CastLob (SpellPower power)
    {
        var cost = LobCosts[power];
        
        if (Mana < cost)
        {
            CantDoThatFeedback.Instance.DisplayMessage("not enough mana!");
            return;
        }
        Mana -= cost;

        var bullet = Instantiate(LobPrefab, transform.position, Quaternion.identity);
        bullet.Initialize(FacingLeft, Color, power);
    }

    public void LongJump ()
    {
        if (isGrounded())
        {
            specialJumping = true;
            rb.velocity = new Vector2
            (
                LongJumpBurst.x * (FacingLeft ? -1 : 1),
                LongJumpBurst.y
            );
        }
    }

    public void HighJump ()
    {
        if (isGrounded())
        { 
            specialJumping = true;
            rb.velocity = new Vector2
            (
                HighJumpBurst.x * (FacingLeft ? -1 : 1),
                HighJumpBurst.y
            );
        }
    }

    // returns whether the cast was successful
    public void Special1 ()
    {
        var cost = getCosts().Special1;
        if (Health <= cost)
        {
            CantDoThatFeedback.Instance.DisplayMessage("not enough health!");
            return;
        }
        
        Health -= cost;

        switch (Color)
        {
            case MagicColor.Red:
                // TODO: nimbility
                break;

            case MagicColor.Green:
                // TODO: rejuve
                break;

            case MagicColor.Blue:
                // TODO: embank
                break;
        }
    }

    // returns whether the cast was successful
    public void Special2 ()
    {
        var cost = getCosts().Special2;
        if (Health <= cost)
        {
            CantDoThatFeedback.Instance.DisplayMessage("not enough health!");
            return;
        }

        Health -= cost;

        switch (Color)
        {
            case MagicColor.Red:
                bombash();
                break;

            case MagicColor.Green:
                recoup();
                break;

            case MagicColor.Blue:
                timeStop();
                break;
        }

    }

    void bombash ()
    {

    }

    void recoup ()
    {

    }

    void timeStop ()
    {

    }

    AbilityCosts getCosts ()
    {
        switch (Color)
        {
            case MagicColor.Red:
                return RedAbilityCosts;

            case MagicColor.Green:
                return GreenAbilityCosts;

            case MagicColor.Blue:
                return BlueAbilityCosts;

            default:
                throw new InvalidOperationException($"Mage has unexpected color {Color}");
        }
    }

    // returns true if there was at least one potion at method call
    public void DrinkHealthPotion ()
    {
        if (MageSquad.Instance.HealthPots == 0)
        {
            CantDoThatFeedback.Instance.DisplayMessage("not enough band aids!");
            return;
        }

        MageSquad.Instance.HealthPots--;
        Health += MageSquad.Instance.HealthPotGain;
    }

    // returns true if there was at least one potion at method call
    public void DrinkManaPotion ()
    {
        if (MageSquad.Instance.ManaPots == 0)
        {
            CantDoThatFeedback.Instance.DisplayMessage("not enough mana potions!");
        }

        MageSquad.Instance.ManaPots--;
        Health += MageSquad.Instance.ManaPotGain;
    }

    void die ()
    {
        Dead = true;
        setAlpha(.5f);

        MageSquad.Instance.ActiveMage = null;
        foreach (var mage in MageSquad.Instance)
        {
            if (!mage.Dead) MageSquad.Instance.ActiveMage = mage;
        }

        if (MageSquad.Instance.ActiveMage == null)
        {
            GameOver.Instance.StartSequence();
        }
    }

    void revive ()
    {
        Dead = false;
        setAlpha(1);
    }

    void setAlpha (float alpha)
    {
        var sr = Visuals.GetComponent<SpriteRenderer>();
        var col = sr.color;
        col.a = alpha;
        sr.color = col;
    }

    void platform ()
    {
        active = (MageSquad.Instance.ActiveMage == this) && !specialJumping;

        moveInput = active ? Input.GetAxisRaw("Move") : 0;
        bool jumpHold = active ? Input.GetButton("Jump") : false;
        if (active && Input.GetButtonDown("Jump"))
        {
            timeSinceLastJumpPress = 0;
        }
        
        if (isGrounded())
        {
            specialJumping = false;

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

            if (active && !jumpHold && newY > JumpSpeedCut)
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

public enum SpellPower
{
    Light, Normal, Heavy
}
