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

        if (EventSystemCache.Main.currentSelectedGameObject == null)
        {
            Debug.LogWarning("no selected game object. should see this message for one frame every time the screen is clicked away from the buttons. if more, there's an issue");
            return;
        }

        transform.position = EventSystemCache.Main.currentSelectedGameObject.transform.position + Vector3.right * HorizontalOffset;
    }
}
