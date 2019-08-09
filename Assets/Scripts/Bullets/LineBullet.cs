using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBullet : BaseMageBullet
{
    [Header("Stats")]
    [Header("Line")]
    public float Speed;

    public void Initialize (bool goingLeft, MagicColor color)
    {
        base.Initialize(color);

        rb.velocity = (goingLeft ? Vector3.left : Vector3.right) * Speed;
    }
}
