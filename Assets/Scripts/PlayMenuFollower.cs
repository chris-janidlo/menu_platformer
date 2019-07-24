using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class PlayMenuFollower : MonoBehaviour
{
    public float FollowTime;
    public Vector2 TopRightLimit;

    Vector3 worldLimit;
    Vector2 velocity;

    void Start ()
    {
        // assuming parent doesn't change
        worldLimit = transform.TransformPoint(TopRightLimit);
    }

    void Update ()
    {
        var target = CameraCache.Main.WorldToScreenPoint(MageSquad.Instance.ActiveMage.transform.position);
        target.x = Mathf.Min(target.x, worldLimit.x);
        target.y = Mathf.Min(target.y, worldLimit.y);

        transform.position = Vector2.SmoothDamp(transform.position, target, ref velocity, FollowTime);
    }
}
