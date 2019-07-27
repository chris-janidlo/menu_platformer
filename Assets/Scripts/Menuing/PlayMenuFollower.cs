using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class PlayMenuFollower : MonoBehaviour
{
    public float FollowTime;

    public float MinX, MaxX, MaxY;

    Vector2 velocity;

    void Update ()
    {
        var target = Vector2.SmoothDamp(transform.position, MageSquad.Instance.ActiveMage.Visuals.transform.position, ref velocity, FollowTime);

        transform.position = new Vector2
        (
            Mathf.Clamp(target.x, MinX, MaxX),
            Mathf.Min(target.y, MaxY)
        );
    }
}
