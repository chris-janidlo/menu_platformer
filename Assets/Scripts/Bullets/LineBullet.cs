using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBullet : BaseMageBullet
{
    [Header("Stats")]
    [Header("Line")]
    public SpellPowerContainer Speeds;
    public SpellPowerContainer Scales;

    public void Initialize (bool goingLeft, MagicColor color, SpellPower power)
    {
        base.Initialize(color, power);

        rb.velocity = (goingLeft ? Vector3.left : Vector3.right) * Speeds[power];
        setScale(Scales);
    }

    void OnBecameInvisible ()
    {
        Destroy(gameObject);
    }
}
