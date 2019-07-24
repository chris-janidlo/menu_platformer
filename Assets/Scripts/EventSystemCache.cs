using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using crass;

public class EventSystemCache : Singleton<EventSystemCache>
{
    public static EventSystem Main => Instance.EventSystem;

    public EventSystem EventSystem;

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }
}
