using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestroyWhenChildrenInvisible))]
public class LobBullet : BaseMageBullet
{
    [Header("Stats")]
    [Header("Line")]
    public float Gravity;
    public SpellPowerContainer LaunchYs, LaunchSpeeds, Scales;

    DestroyWhenChildrenInvisible destroyInvisible;

    public void Initialize (bool goingLeft, MagicColor color, SpellPower power)
    {
        base.Initialize(color, power);

        rb.velocity = new Vector2
        (
            goingLeft ? -1 : 1,
            LaunchYs[power]
        ).normalized * LaunchSpeeds[power];

        setScale(Scales);

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
