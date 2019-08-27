using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using crass;

public class EventSystemCache : Singleton<EventSystemCache>
{
    public static EventSystem Main => Instance.EventSystem;

    public EventSystem EventSystem;

    GameObject lastSelected;

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }

    void Update ()
    {
        if (EventSystem.currentSelectedGameObject == null)
        {
            EventSystem.SetSelectedGameObject(lastSelected);
        }

        lastSelected = EventSystem.currentSelectedGameObject;
    }

    public void SetSelected (GameObject selected)
    {
        lastSelected = selected;
        EventSystem.SetSelectedGameObject(selected);
    }
}
