using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

[RequireComponent(typeof(Rigidbody2D), typeof(DestroyWhenChildrenInvisible))]
public class Goose : BaseEnemy
{
    public float HorizontalFollowDistance, FollowTime;
    public float InitialHeight, Gravity;
    public float AttackRoutineStartDistance, AttackChargeTime;
    public Vector2 TimeRangeBetweenAttacks, LaserSpawnOffset;

    public GooseLaser LaserPrefab;
    public SpriteRenderer Visuals;

    Transform target;

    Rigidbody2D rb;
    Vector2 vel;

    bool attacking;

    bool facingLeft => transform.position.x > target.position.x;

	public override void Initialize (MagicColor color)
	{
        rb = GetComponent<Rigidbody2D>();

        target = MageSquad.Instance[color].transform;
        transform.position = target.position + Vector3.up * InitialHeight;
	}

    protected override void Awake ()
    {
        GetComponent<DestroyWhenChildrenInvisible>().enabled = false;
    }

    protected override void Update ()
    {
        if (Health.Dead)
        {
            rb.velocity += Vector2.down * Gravity * Time.deltaTime;
        }
        else
        {
            transform.position = Vector2.SmoothDamp(transform.position, getFollowPosition(), ref vel, FollowTime);

            Visuals.flipX = facingLeft;
        }

        if (!attacking && Vector2.Distance(transform.position, target.position) <= AttackRoutineStartDistance)
        {
            attacking = true;
            StartCoroutine(attackRoutine());
        }
    }

	protected override void die ()
	{
        GetComponent<DestroyWhenChildrenInvisible>().enabled = true;
	}

    Vector2 getFollowPosition ()
    {
        return (Vector2) target.position + HorizontalFollowDistance * (facingLeft ? Vector2.right : Vector2.left);
    }

    IEnumerator attackRoutine ()
    {
        while (!Health.Dead)
        {
            yield return new WaitForSeconds(RandomExtra.Range(TimeRangeBetweenAttacks));

            // TODO: show charging animation

            yield return new WaitForSeconds(AttackChargeTime);

            // TODO: show attack animation

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
