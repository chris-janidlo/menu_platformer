using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class PlayMenuFollower : MonoBehaviour
{
    public float FollowTime;

    Vector2 velocity;

    void Update ()
    {
        transform.position = Vector2.SmoothDamp(transform.position, MageSquad.Instance.ActiveMage.Visuals.transform.position, ref velocity, FollowTime);
    }
}
