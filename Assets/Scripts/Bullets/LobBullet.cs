using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestroyWhenChildrenInvisible))]
public class LobBullet : BaseMageBullet
{
    [Header("Stats")]
    [Header("Line")]
    public float Gravity;
    public Vector2 LaunchSpeed;

    DestroyWhenChildrenInvisible destroyInvisible;

    public void Initialize (bool goingLeft, MagicColor color)
    {
        base.Initialize(color);

        rb.velocity = new Vector2
        (
            LaunchSpeed.x * (goingLeft ? -1 : 1),
            LaunchSpeed.y
        );

        destroyInvisible = GetComponent<DestroyWhenChildrenInvisible>();
    }

    void Update ()
    {
        rb.velocity = new Vector2
        (
            rb.velocity.x,
            rb.velocity.y - Gravity * Time.deltaTime
        );

        // don't destroy if the player lobs over the top of the screen
        destroyInvisible.ShouldDestroy = transform.position.y < 0;
    }
}
