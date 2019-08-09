using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(DestroyWhenChildrenInvisible))]
[RequireComponent(typeof(Animator))]
public class Butterfly : BaseEnemy
{
    [Header("Stats")]
    public float MaxAngleDeltaPerSecond;
    public float RoundRotationTo;
    public float FlySpeedNormal, FlySpeedChase, FlySpeedDead;
    public float ChaseDistance;
    public float ChaseAnimationSpeed;
    public float Damage, PostDamageRefractoryPeriod;

    [Header("References")]
    public ColorMapApplier ColoredPart;

    MagicColor color => Health.Color;

    float movementAngle, movementAngleSmoothed, smoothmentSpeed;
    float damageRefractoryTimer;

    Rigidbody2D rb;
    Collider2D col;
    Animator animator;
    DestroyWhenChildrenInvisible destroyer;

    public void Initialize ()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        destroyer = GetComponent<DestroyWhenChildrenInvisible>();

        col.enabled = false; // so we don't freak out while we're in the walls
        
        destroyer.ShouldDestroy = false;

        var color = (MagicColor) Random.Range(0, 3);

        Health.Color = color;
        ColoredPart.ChangeColor(color);

        transform.position = EnemySpawner.Instance.ButterflySpawnLocations.GetNext();
    }

    protected override void Update ()
    {
        base.Update();

        if (Health.Dead)
        {
            // fly away from center
            rb.velocity = transform.position.normalized * FlySpeedDead;
            return;
        }

        if (destroyer.ChildrenInvisible)
        {
            // fly toward center
            rb.velocity = (Vector3.zero - transform.position).normalized * FlySpeedNormal;
            
            // face the center
            if (Mathf.Abs(rb.velocity.y) > Mathf.Abs(rb.velocity.x))
            {
                movementAngle = -90;
                movementAngleSmoothed = -90;
            }
            else if (rb.velocity.x > 0)
            {
                movementAngle = 180;
                movementAngleSmoothed = 180;
            }
            // else movementAngle = 0;

            return;
        }
        else if (!col.enabled)
        {
            col.enabled = true;
        }

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

        // smooth the value by making the actually used angle take longer to catch up with the internal angle the closer they are. also provide a minimum value that's greater than zero to avoid divide by zero
        var smoother = 1 / Mathf.Max(Mathf.Epsilon, Mathf.Abs(movementAngle - movementAngleSmoothed));
        movementAngleSmoothed = Mathf.SmoothDamp(movementAngleSmoothed, movementAngle, ref smoothmentSpeed, smoother);

        var movementAngleRounded = Mathf.Round(movementAngleSmoothed / RoundRotationTo) * RoundRotationTo;

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
		col.enabled = false;
        destroyer.ShouldDestroy = true;
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
