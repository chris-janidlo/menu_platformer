using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class PlayMenuFollower : MonoBehaviour
{
    // 174, 115
    public Vector2 TopRightLimit;

    Vector3 worldLimit;

    void Start ()
    {
        worldLimit = transform.TransformPoint(TopRightLimit);
    }

    void Update ()
    {
        var target = CameraCache.Main.WorldToScreenPoint(MageSquad.Instance.ActiveMage.transform.position);
        target.x = Mathf.Min(target.x, worldLimit.x);
        target.y = Mathf.Min(target.y, worldLimit.y);
        transform.position = target;
    }
}
