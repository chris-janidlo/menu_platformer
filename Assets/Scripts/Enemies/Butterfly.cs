using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(DestroyWhenChildrenInvisible))]
public class Butterfly : BaseEnemy
{
    [Header("Stats")]
    public float FlySpeed;
    public float MaxAngleDeltaPerSecond;
    public Vector2 BottomLeftCorner, TopRightCorner;

    [Header("References")]
    public ColorMapApplier ColoredPart;

    MagicColor color => Health.Color;

    Rigidbody2D rb;

    [SerializeField]
    float movementAngle;

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

    protected override void Update ()
    {
        base.Update();

        float deltaPerSec = MaxAngleDeltaPerSecond * Time.deltaTime;
        movementAngle += Random.Range(-deltaPerSec, deltaPerSec);

        if (transform.position.x < BottomLeftCorner.x || transform.position.y < BottomLeftCorner.y || transform.position.x > TopRightCorner.x || transform.position.y > TopRightCorner.y)
        {
            movementAngle += 180;
        }

        // keep angle in range (0, 360) so it doesn't get too huge
        movementAngle = Mathf.Repeat(movementAngle, 360);

        rb.velocity = Quaternion.AngleAxis(movementAngle, Vector3.forward) * Vector2.right * FlySpeed;
        rb.rotation = movementAngle;
    }

	protected override void die ()
	{
		throw new System.NotImplementedException();
	}
}
