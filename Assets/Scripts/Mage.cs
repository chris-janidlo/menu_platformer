using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Mage : MonoBehaviour
{
    public float MoveSpeed;
    public float AirAcceleration;
    public float JumpSpeedBurst, JumpSpeedCut;
    public float Gravity;
    public float GroundedFudgeVertical = 0.1f, GroundedFudgeHorizontal = 0.1f;

    Rigidbody2D rb;
    float halfHeight;
    Vector2 groundedExtents;
    bool jumping;

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
        float move = Input.GetAxisRaw("Move");
        bool jump = Input.GetButton("Jump");
        
        if (isGrounded())
        {
            if (!jump)
            {
                rb.velocity = Vector2.right * MoveSpeed * move;
                jumping = false;
            }
            else if (!jumping)
            {
                rb.velocity = new Vector3(rb.velocity.x, JumpSpeedBurst);
                jumping = true;
            }
        }
        else
        {
            var newX = rb.velocity.x + move * AirAcceleration * Time.deltaTime;
            newX = Mathf.Clamp(newX, -MoveSpeed, MoveSpeed);

            var newY = rb.velocity.y - Gravity * Time.deltaTime;

            if (!jump && newY > JumpSpeedCut)
            {
                newY = JumpSpeedCut;
            }

            rb.velocity = new Vector2(newX, newY);
        }
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
