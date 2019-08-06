using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using crass;

public class StartMenuSelectionFollower : Singleton<StartMenuSelectionFollower>
{
    public float HorizontalOffset;

    public EventSystem EventSystem;

    GameObject lastSelected;

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }

    void Update ()
    {
        if (Time.time == 0) return; // wait a frame so the event system can select the first object

        if (EventSystem.currentSelectedGameObject == null)
        {
            EventSystem.SetSelectedGameObject(lastSelected);
        }

        lastSelected = EventSystem.currentSelectedGameObject;

        transform.position = EventSystem.currentSelectedGameObject.transform.position + Vector3.right * HorizontalOffset;
    }
}
