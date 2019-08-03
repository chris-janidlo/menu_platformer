using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

[RequireComponent(typeof(Rigidbody2D), typeof(DestroyWhenChildrenInvisible))]
public class Goose : BaseEnemy
{
    public float HorizontalFollowDistance, FollowTime;
    public float InitialHeight, Gravity;
    public float AttackChargeTime;
    public Vector2 TimeRangeBetweenAttacks;

    public GooseLaser LaserPrefab;

    Mage target;

    Rigidbody2D rb;
    Vector2 vel;

	public override void Initialize (MagicColor color)
	{
        rb = GetComponent<Rigidbody2D>();

        target = MageSquad.Instance[color];
        transform.position = target.transform.position + Vector3.up * InitialHeight;
	}

    protected override void Awake ()
    {
        GetComponent<DestroyWhenChildrenInvisible>().enabled = false;
    }

    void Start ()
    {
        Initialize(MagicColor.Red);
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
        }
    }

	protected override void die ()
	{
        GetComponent<DestroyWhenChildrenInvisible>().enabled = true;
	}

    Vector2 getFollowPosition ()
    {
        Vector2 magePos = target.transform.position;

        int direction = transform.position.x < magePos.x ? -1 : 1;

        return magePos + Vector2.right * HorizontalFollowDistance * direction;
    }

    IEnumerator attackRoutine ()
    {
        while (!Health.Dead)
        {
            yield return new WaitForSeconds(RandomExtra.Range(TimeRangeBetweenAttacks));

            // TODO: show charging animation

            yield return new WaitForSeconds(AttackChargeTime);

            // TODO: show attack animation
            
            Vector2 direction = (target.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Instantiate(LaserPrefab, transform.position, Quaternion.AngleAxis(angle, Vector3.forward)).Initialize(direction);
        }
    }
}
