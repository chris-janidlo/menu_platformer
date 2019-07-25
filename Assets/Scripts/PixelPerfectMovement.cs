using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// objects that use this should have a parent object for gameplay/physics and a child object strictly for visuals. put this on the child
public class PixelPerfectMovement : MonoBehaviour
{
    void LateUpdate ()
    {
        transform.position = new Vector3
        (
            Mathf.Round(transform.parent.position.x),
            Mathf.Round(transform.parent.position.y),
            Mathf.Round(transform.parent.position.z)
        );
    }
}
