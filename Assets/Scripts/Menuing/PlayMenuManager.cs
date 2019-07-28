using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using crass;

public class PlayMenuManager : Singleton<PlayMenuManager>
{
    public bool MenuIsActive { get; private set; }

    public GameObject MenuContainer;
    public Button FirstTopLevelButton;

    // bottom-level buttons
    public Button BurstNormal, BurstHeavy, BurstLight, LineNormal, LineHeavy,
        LineLight, LobNormal, LobHeavy, LobLight, RedMage, GreenMage, BlueMage,
        LongJump, HighJump, Special1, Special2, ManaPot, HealthPot;

    GameObject lastSelected;

    bool gameStarted;

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }

    void Start ()
    {
        BurstNormal.onClick.AddListener(() => MageSquad.Instance.ActiveMage.CastBurst(SpellPower.Normal));
        BurstHeavy.onClick.AddListener(() => MageSquad.Instance.ActiveMage.CastBurst(SpellPower.Heavy));
        BurstLight.onClick.AddListener(() => MageSquad.Instance.ActiveMage.CastBurst(SpellPower.Light));

        LineNormal.onClick.AddListener(() => MageSquad.Instance.ActiveMage.CastLine(SpellPower.Normal));
        LineHeavy.onClick.AddListener(() => MageSquad.Instance.ActiveMage.CastLine(SpellPower.Heavy));
        LineLight.onClick.AddListener(() => MageSquad.Instance.ActiveMage.CastLine(SpellPower.Light));

        LobNormal.onClick.AddListener(() => MageSquad.Instance.ActiveMage.CastLob(SpellPower.Normal));
        LobHeavy.onClick.AddListener(() => MageSquad.Instance.ActiveMage.CastLob(SpellPower.Heavy));
        LobLight.onClick.AddListener(() => MageSquad.Instance.ActiveMage.CastLob(SpellPower.Light));

        RedMage.onClick.AddListener(() => MageSquad.Instance.SetActive(MagicColor.Red));
        GreenMage.onClick.AddListener(() => MageSquad.Instance.SetActive(MagicColor.Green));
        BlueMage.onClick.AddListener(() => MageSquad.Instance.SetActive(MagicColor.Blue));

        LongJump.onClick.AddListener(() => MageSquad.Instance.ActiveMage.LongJump());
        HighJump.onClick.AddListener(() => MageSquad.Instance.ActiveMage.HighJump());

        Special1.onClick.AddListener(() => MageSquad.Instance.ActiveMage.Special1());
        Special2.onClick.AddListener(() => MageSquad.Instance.ActiveMage.Special2());

        ManaPot.onClick.AddListener(() => MageSquad.Instance.ActiveMage.DrinkManaPotion());
        HealthPot.onClick.AddListener(() => MageSquad.Instance.ActiveMage.DrinkHealthPotion());

        GetComponent<PlayMenuFollower>().enabled = false;
    }

    void Update ()
    {
        var selected = EventSystemCache.Main.currentSelectedGameObject;
        if (selected == null)
        {
            EventSystemCache.Main.SetSelectedGameObject(lastSelected); // prevent clicking away to deselect
        }
        else
        {
            lastSelected = selected;
        }

        if (!gameStarted) return;

        bool visible = MenuContainer.activeSelf;
        bool atTop = FirstTopLevelButton.interactable;

        if (!visible && Input.GetButtonDown("Menu Select"))
        {
            MenuContainer.SetActive(true);
            StartCoroutine(reHighlightButton());
            StartCoroutine(setActiveNextFrame());
        }

        if (atTop && Input.GetButtonDown("Menu Cancel"))
        {
            MenuContainer.SetActive(false);
            MenuIsActive = false;
        }

        if (Input.GetButtonDown("Menu Open"))
        {
            MenuContainer.SetActive(!visible);

            if (!visible)
            {
                StartCoroutine(reHighlightButton());
                StartCoroutine(setActiveNextFrame());
            }
            else
            {
                MenuIsActive = false;
            }
        }
    }

    public void StartGame ()
    {
        gameStarted = true;

        MenuContainer.SetActive(true);
        MenuIsActive = true;

        lastSelected = FirstTopLevelButton.gameObject;
        StartCoroutine(reHighlightButton());

        GetComponent<PlayMenuFollower>().enabled = true;
    }

    // prevents menu opening press from triggering currently highlighted button by making the submenuselectors wait at least one frame
    IEnumerator setActiveNextFrame ()
    {
        yield return null;
        MenuIsActive = true;
    }

    // from https://answers.unity.com/questions/1011523/first-selected-gameobject-not-highlighted.html
    IEnumerator reHighlightButton ()
    {
        EventSystemCache.Main.SetSelectedGameObject(null);
        yield return null;
        EventSystemCache.Main.SetSelectedGameObject(lastSelected);
    }
}
