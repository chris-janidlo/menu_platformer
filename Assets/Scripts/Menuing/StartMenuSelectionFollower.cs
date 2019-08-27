using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
        if (EventSystemCache.Main.currentSelectedGameObject == null) return;

        transform.position = EventSystemCache.Main.currentSelectedGameObject.transform.position + Vector3.left * HorizontalOffset;
    }
}
