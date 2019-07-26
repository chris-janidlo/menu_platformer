using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class StartMenuSelectionFollower : Singleton<StartMenuSelectionFollower>
{
    public float HorizontalOffset;

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }

    void Update ()
    {
        if (Time.time == 0) return; // wait a frame so the event system can select the first object

        transform.position = EventSystemCache.Main.currentSelectedGameObject.transform.position + Vector3.right * HorizontalOffset;
    }
}
