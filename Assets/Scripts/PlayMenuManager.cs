using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using crass;

public class PlayMenuManager : Singleton<PlayMenuManager>
{
    public GameObject MenuContainer;
    public Button AnyTopLevelButton;

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
        bool visible = MenuContainer.activeSelf;
        bool atTop = AnyTopLevelButton.interactable;

        if (!visible && Input.GetButtonDown("Menu Select"))
        {
            MenuContainer.SetActive(true);
            StartCoroutine(reHighlightButton());
        }

        if (atTop && Input.GetButtonDown("Menu Cancel"))
        {
            MenuContainer.SetActive(false);
        }

        if (Input.GetButtonDown("Menu Open"))
        {
            if (!visible) StartCoroutine(reHighlightButton());
            MenuContainer.SetActive(!visible);
        }

        var selected = EventSystemCache.Main.currentSelectedGameObject;
        if (selected == null)
        {
            EventSystemCache.Main.SetSelectedGameObject(lastSelected); // prevent clicking away to deselect
        }
        else
        {
            lastSelected = selected;
        }
    }

    // from https://answers.unity.com/questions/1011523/first-selected-gameobject-not-highlighted.html
    IEnumerator reHighlightButton ()
    {
        EventSystemCache.Main.SetSelectedGameObject(null);
        yield return null;
        EventSystemCache.Main.SetSelectedGameObject(lastSelected);
    }
}
