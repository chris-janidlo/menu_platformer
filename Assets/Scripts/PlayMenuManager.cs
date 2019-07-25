using System;
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

    void Start ()
    {
        spellscribe(BurstNormal, () => MageSquad.Instance.ActiveMage.CastBurst(SpellPower.Normal));
        spellscribe(BurstHeavy, () => MageSquad.Instance.ActiveMage.CastBurst(SpellPower.Heavy));
        spellscribe(BurstLight, () => MageSquad.Instance.ActiveMage.CastBurst(SpellPower.Light));

        spellscribe(LineNormal, () => MageSquad.Instance.ActiveMage.CastLine(SpellPower.Normal));
        spellscribe(LineHeavy, () => MageSquad.Instance.ActiveMage.CastLine(SpellPower.Heavy));
        spellscribe(LineLight, () => MageSquad.Instance.ActiveMage.CastLine(SpellPower.Light));

        spellscribe(LobNormal, () => MageSquad.Instance.ActiveMage.CastLob(SpellPower.Normal));
        spellscribe(LobHeavy, () => MageSquad.Instance.ActiveMage.CastLob(SpellPower.Heavy));
        spellscribe(LobLight, () => MageSquad.Instance.ActiveMage.CastLob(SpellPower.Light));

        RedMage.onClick.AddListener(() => MageSquad.Instance.SetActive(MagicColor.Red));
        GreenMage.onClick.AddListener(() => MageSquad.Instance.SetActive(MagicColor.Green));
        BlueMage.onClick.AddListener(() => MageSquad.Instance.SetActive(MagicColor.Blue));

        LongJump.onClick.AddListener(() => MageSquad.Instance.ActiveMage.LongJump());
        HighJump.onClick.AddListener(() => MageSquad.Instance.ActiveMage.HighJump());

        spellscribe(Special1, () => MageSquad.Instance.ActiveMage.Special1());
        spellscribe(Special2, () => MageSquad.Instance.ActiveMage.Special2());

        spellscribe(ManaPot, () => MageSquad.Instance.ActiveMage.DrinkManaPotion(), "not enough items!");
        spellscribe(HealthPot, () => MageSquad.Instance.ActiveMage.DrinkHealthPotion(), "not enough items!");
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

    void spellscribe (Button button, Func<bool> spell, string message = "not enough mana!")
    {
        button.onClick.AddListener(() => {
            if (!spell())
            {
                Debug.Log(message);
            }
        });
    }

    // from https://answers.unity.com/questions/1011523/first-selected-gameobject-not-highlighted.html
    IEnumerator reHighlightButton ()
    {
        EventSystemCache.Main.SetSelectedGameObject(null);
        yield return null;
        EventSystemCache.Main.SetSelectedGameObject(lastSelected);
    }
}
