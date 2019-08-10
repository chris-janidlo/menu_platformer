using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(DestroyWhenChildrenInvisible))]
public class Goose : BaseEnemy
{
    public float HorizontalFollowDistance, FollowTime;
    public float InitialHeight, Gravity;
    public float AttackRoutineStartDistance, AttackChargeTime;
    public float SpeedConsideredFalling;
    public float ShakeMagnitudeMax;
    public Vector2 TimeRangeBetweenAttacks, LaserSpawnOffset;

    public GooseLaser LaserPrefab;
    public SpriteRenderer Visuals;

    Transform target;

    Animator animator;
    Rigidbody2D rb;
    Vector2 smoothDampVel, previousPosition;

    bool following;

    bool facingLeft => transform.position.x > target.position.x;

    bool attacking
    {
        get => animator.GetBool("Attacking");
        set => animator.SetBool("Attacking", value);
    }

	public void Initialize (Transform target)
	{
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        this.target = target;
        transform.position = target.position + Vector3.up * InitialHeight;
	}

    protected override void Update ()
    {
        if (Health.Dead)
        {
            rb.velocity += Vector2.down * Gravity * Time.deltaTime;
        }
        else
        {
            float followTime = FollowTime ;
            
            if (isFrozen)
            {
                // lengthen follow time by the inverse amount of slow
                // eg: if slow percent is 80%, this will make the follow time 120% of what it normally is (think of it as "20% slower")
                followTime *= 2 - BaseMageBullet.IceSlowPercent;
            }

            transform.position = Vector2.SmoothDamp(transform.position, getFollowPosition(), ref smoothDampVel, followTime);

            if (attacking) transform.position += (Vector3) Random.insideUnitCircle * ShakeMagnitudeMax;

            Visuals.flipX = facingLeft;
        }

        Vector2 velocity = ((Vector2) transform.position - previousPosition) / Time.deltaTime;
        animator.SetBool("Falling", velocity.y <= -SpeedConsideredFalling);
        previousPosition = transform.position;

        if (!following && Vector2.Distance(transform.position, target.position) <= AttackRoutineStartDistance)
        {
            following = true;
            StartCoroutine(attackRoutine());
        }
    }

	protected override void die ()
	{
        GetComponent<DestroyWhenChildrenInvisible>().ShouldDestroy = true;

        StopAllCoroutines();
	}

    Vector2 getFollowPosition ()
    {
        return (Vector2) target.position + HorizontalFollowDistance * (facingLeft ? Vector2.right : Vector2.left);
    }

    IEnumerator attackRoutine ()
    {
        while (true)
        {
            yield return new WaitForSeconds(RandomExtra.Range(TimeRangeBetweenAttacks));

            attacking = true;

            yield return new WaitForSeconds(AttackChargeTime);

            attacking = false;

            Vector3 laserPos = transform.position + new Vector3
            (
                LaserSpawnOffset.x * (facingLeft ? -1 : 1),
                LaserSpawnOffset.y
            );
            
            Vector2 direction = (target.position - laserPos).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Instantiate(LaserPrefab, laserPos, Quaternion.AngleAxis(angle, Vector3.forward)).Initialize(direction);
        }
    }
}
