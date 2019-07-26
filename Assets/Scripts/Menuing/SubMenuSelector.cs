﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class SubMenuSelector : MonoBehaviour
{
    public const int SubMenuVerticalPlay = 5;

    public Button FirstSelectedChild;
    // siblings can include self or not, either way works
    public List<Button> Siblings, Children;
    public RectTransform ChildContainer;

    Button mainButton;
    bool subMenuActive;

    void Start ()
    {
        mainButton = GetComponent<Button>();
        mainButton.onClick.AddListener(click);
    }

	public void Update ()
	{
        if (Input.GetButtonDown("Menu Cancel") && subMenuActive && FirstSelectedChild.interactable)
        {
            setSubMenuState(false);
        }
	}

    void click ()
    {
        setSubMenuState(true);

        ChildContainer.transform.localPosition = new Vector2
        (
            ChildContainer.transform.localPosition.x,
            Random.Range(-SubMenuVerticalPlay, SubMenuVerticalPlay)
        );
    }

    void setSubMenuState (bool value)
    {
        mainButton.interactable = !value;

        foreach (var sibling in Siblings)
        {
            sibling.interactable = !value;
        }

        foreach (var child in Children)
        {
            child.interactable = value;
        }

        ChildContainer.gameObject.SetActive(value);

        EventSystemCache.Main.SetSelectedGameObject(value ? FirstSelectedChild.gameObject : mainButton.gameObject);

        subMenuActive = value;
    }
}