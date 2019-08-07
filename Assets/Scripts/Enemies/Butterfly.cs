using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(DestroyWhenChildrenInvisible))]
public class Butterfly : BaseEnemy
{
    [Header("Stats")]
    public float MaxAngleDeltaPerSecond;
    public float FlySpeedNormal, FlySpeedChase;
    public float ChaseDistance;
    public float Damage, PostDamageRefractoryPeriod;
    public Vector2 BottomLeftCorner, TopRightCorner;

    [Header("References")]
    public ColorMapApplier ColoredPart;

    MagicColor color => Health.Color;

    Rigidbody2D rb;

    [SerializeField]
    float movementAngle;

    float damageRefractoryTimer;

    public void Initialize (MagicColor color)
    {
        rb = GetComponent<Rigidbody2D>();

        Health.Color = color;
        ColoredPart.ChangeColor(color);
    }

    void Start ()
    {
        Initialize(MagicColor.Red);
    }

    // TODO: ice movement
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

        // bounce off screen walls
        if (transform.position.x < BottomLeftCorner.x || transform.position.y < BottomLeftCorner.y || transform.position.x > TopRightCorner.x || transform.position.y > TopRightCorner.y)
        {
            movementAngle += 180;
        }

        // keep angle in range (0, 360) so it doesn't overflow
        movementAngle = Mathf.Repeat(movementAngle, 360);

        var flySpeed = chasing ? FlySpeedChase : FlySpeedNormal;

        rb.velocity = Quaternion.AngleAxis(movementAngle, Vector3.forward) * Vector2.right * flySpeed;
        rb.rotation = movementAngle;
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (damageRefractoryTimer >= 0) return;

        Mage mage = other.GetComponent<Mage>();
        if (mage == null) return;

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
