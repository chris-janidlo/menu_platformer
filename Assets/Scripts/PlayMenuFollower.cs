using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class PlayMenuFollower : MonoBehaviour
{
    void Update ()
    {
        transform.position = CameraCache.Main.WorldToScreenPoint(MageSquad.Instance.ActiveMage.transform.position);
    }
}
