using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ColorMapApplier))]
public class Mage : MonoBehaviour
{
    public MagicColor Color => GetComponent<ColorMapApplier>().Color;

    public float MoveSpeed;
    public float AirAcceleration;
    public float JumpSpeedBurst, JumpSpeedCut;
    public float Gravity;
    public float JumpFudgeTime;
    public float GroundedFudgeVertical = 0.1f, GroundedFudgeHorizontal = 0.1f;

    Rigidbody2D rb;
    float halfHeight;
    Vector2 groundedExtents;
    float timeSinceLastJumpPress = float.MaxValue;

    RaycastHit2D[] groundedResults;
    ContactFilter2D groundedFilter;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();

        var extents = GetComponent<Collider2D>().bounds.extents;
        halfHeight = extents.y;
        groundedExtents = new Vector2
        (
            extents.x + GroundedFudgeHorizontal / 2,
            GroundedFudgeVertical / 2
        );

        groundedFilter = new ContactFilter2D();
        groundedFilter.NoFilter();
    }

    void Update ()
    {
        bool active = (MageSquad.Instance.ActiveMage == this);

        float move = active ? Input.GetAxisRaw("Move") : 0;
        bool jumpHold = active ? Input.GetButton("Jump") : false;
        if (active && Input.GetButtonDown("Jump"))
        {
            timeSinceLastJumpPress = 0;
        }
        
        if (isGrounded())
        {
            rb.velocity = Vector2.right * MoveSpeed * move;

            if (timeSinceLastJumpPress <= JumpFudgeTime)
            {
                rb.velocity = new Vector3(rb.velocity.x, JumpSpeedBurst);
            }
        }
        else
        {
            var newX = rb.velocity.x + move * AirAcceleration * Time.deltaTime;
            newX = Mathf.Clamp(newX, -MoveSpeed, MoveSpeed);

            var newY = rb.velocity.y - Gravity * Time.deltaTime;

            if (!jumpHold && newY > JumpSpeedCut)
            {
                newY = JumpSpeedCut;
            }

            rb.velocity = new Vector2(newX, newY);
        }

        timeSinceLastJumpPress += Time.deltaTime;
    }

    bool isGrounded ()
    {
        groundedResults = new RaycastHit2D[2]; // don't need more than two; one for us, one for the ground
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
