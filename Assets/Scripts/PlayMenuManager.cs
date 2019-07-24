using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using crass;

public class PlayMenuManager : Singleton<PlayMenuManager>
{
    public EventSystem EventSystem;

    // bottom-level buttons
    public Button BurstNormal, BurstHeavy, BurstLight, LineNormal, LineHeavy,
        LineLight, LobNormal, LobHeavy, LobLight, RedMage, GreenMage, BlueMage,
        LongJump, HighJump, Special1, Special2, ManaPot, HealthPot;

    GameObject lastSelected;

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }

    void Update ()
    {
        // if (/*don't care about last selected*/) return;
        var selected = EventSystem.currentSelectedGameObject;
        if (selected == null)
        {
            EventSystem.SetSelectedGameObject(lastSelected); // prevent clicking away to deselect
        }
        else
        {
            lastSelected = selected;
        }
    }
}
