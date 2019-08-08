using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(DestroyWhenChildrenInvisible))]
[RequireComponent(typeof(Animator))]
public class Butterfly : BaseEnemy
{
    [Header("Stats")]
    public float MaxAngleDeltaPerSecond;
    public float DeltaDeltaPerSecond;
    public float RoundRotationTo;
    public float FlySpeedNormal, FlySpeedChase;
    public float ChaseDistance;
    public float ChaseAnimationSpeed;
    public float Damage, PostDamageRefractoryPeriod;

    [Header("References")]
    public ColorMapApplier ColoredPart;

    MagicColor color => Health.Color;

    [SerializeField]
    float movementAngle, currentDelta;
    float damageRefractoryTimer;

    Rigidbody2D rb;
    Animator animator;

    public void Initialize (MagicColor color)
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        Health.Color = color;
        ColoredPart.ChangeColor(color);
    }

    void Start ()
    {
        Initialize(MagicColor.Green);
    }

    protected override void Update ()
    {
        base.Update();

        damageRefractoryTimer -= Time.deltaTime;

        bool chasing;
        float deltaPerSec = MaxAngleDeltaPerSecond * Time.deltaTime;

        Mage mage;
        if (damageRefractoryTimer < 0 && distanceToClosestLivingMage(out mage) < ChaseDistance)
        {
            var delta = Random.Range(0, deltaPerSec);

            movementAngle += Vector2.SignedAngle(mage.transform.position - transform.position, transform.right) > 0
                ? -delta
                : delta;

            chasing = true;
        }
        else
        {
            movementAngle += Random.Range(-deltaPerSec, deltaPerSec);
            chasing = false;
        }

        // keep angle in range (0, 360) so it doesn't overflow
        movementAngle = Mathf.Repeat(movementAngle, 360);

        var movementAngleRounded = Mathf.Round(movementAngle / RoundRotationTo) * RoundRotationTo;

        var flySpeed =
            (chasing ? FlySpeedChase : FlySpeedNormal) *
            (isFrozen ? BaseMageBullet.IceSlowPercent : 1);

        rb.velocity = Quaternion.AngleAxis(movementAngleRounded, Vector3.forward) * Vector2.right * flySpeed;
        rb.rotation = movementAngleRounded;

        animator.speed = chasing ? ChaseAnimationSpeed : 1;
    }

    void OnCollisionEnter2D (Collision2D other)
    {
        // bounce
        movementAngle += 180;

        if (damageRefractoryTimer >= 0) return;

        Mage mage = other.gameObject.GetComponent<Mage>();
        if (mage == null || mage.Health.Dead) return;

        mage.Health.ColorDamage(Damage, color);
        damageRefractoryTimer = PostDamageRefractoryPeriod;
    }

	protected override void die ()
	{
		throw new System.NotImplementedException();
	}

    float distanceToClosestLivingMage (out Mage mage)
    {
        float dist = float.MaxValue;
        mage = MageSquad.Instance[MagicColor.Red]; // need to assign outside of an if statement in order for this to compile because computer dumb
        
        foreach (var m in MageSquad.Instance)
        {
            var qdist = Vector2.Distance(transform.position, m.transform.position);

            if (!m.Health.Dead && qdist < dist)
            {
                dist = qdist;
                mage = m;
            }
        }

        return dist;
    }
}
