using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobBullet : BaseMageBullet
{
    [Header("Stats")]
    [Header("Line")]
    public float Gravity;
    public SpellPowerContainer LaunchYs, LaunchSpeeds, Scales;

    public void Initialize (bool goingLeft, MagicColor color, SpellPower power)
    {
        base.Initialize(color, power);

        rb.velocity = new Vector2
        (
            goingLeft ? -1 : 1,
            LaunchYs[power]
        ).normalized * LaunchSpeeds[power];

        setScale(Scales);
    }

    void Update ()
    {
        rb.velocity = new Vector2
        (
            rb.velocity.x,
            rb.velocity.y - Gravity * Time.deltaTime
        );
    }

    void OnBecameInvisible ()
    {
        // let players lob off the top of the screen
        if (transform.position.y < 0) Destroy(gameObject);
    }
}
