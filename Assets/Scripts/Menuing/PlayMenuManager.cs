using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using crass;

public class PlayMenuManager : Singleton<PlayMenuManager>
{
    public float MenuAnimationInitialOffset, MenuAnimationTime;
    public string MenuEntrySeparator;
    public TextMeshProUGUI MenuEntries;

    PlayMenuInternalNode tree = new PlayMenuInternalNode
    (
        "_top",
        new PlayMenuInternalNode
        (
            "Magic",
            new PlayMenuLeafNode
            (
                "Burst",
                activeMage => activeMage.CastBurst(SpellPower.Normal)
            ),
            new PlayMenuLeafNode
            (
                "Shoot",
                activeMage => activeMage.CastLine(SpellPower.Normal)
                
            ),
            new PlayMenuLeafNode
            (
                "Lob",
                activeMage => activeMage.CastLob(SpellPower.Normal)
                
            )
        ),
        new PlayMenuInternalNode
        (
            "Team",
            new PlayMenuLeafNode
            (
                "Red Mage",
                () => MageSquad.Instance.SetActive(MagicColor.Red)
            ),
            new PlayMenuLeafNode
            (
                "Green Mage",
                () => MageSquad.Instance.SetActive(MagicColor.Green)
            ),
            new PlayMenuLeafNode
            (
                "Blue Mage",
                () => MageSquad.Instance.SetActive(MagicColor.Blue)
            )
        ),
        new PlayMenuInternalNode
        (
            "Movement",
            new PlayMenuLeafNode
            (
                "High Jump",
                activeMage => activeMage.HighJump()
            ),
            new PlayMenuLeafNode
            (
                "Long Jump",
                activeMage => activeMage.LongJump()
            )
        ),
        new PlayMenuInternalNode
        (
            "Blood Magic",
            new PlayMenuLeafNode
            (
                "special1",
                activeMage => activeMage.Special1()
            ),
            new PlayMenuLeafNode
            (
                "special2",
                activeMage => activeMage.Special2()
            )
        ),
        new PlayMenuInternalNode
        (
            "Item",
            new PlayMenuLeafNode
            (
                "Band-Aid",
                activeMage => activeMage.DrinkHealthPotion()
            ),
            new PlayMenuLeafNode
            (
                "Mana Pot",
                activeMage => activeMage.DrinkManaPotion()
            )
        )
    );

    // currentlySelected is duplicated as a reminder to never set it directly; instead use setSelected
    PlayMenuNode _currentlySelected;
    PlayMenuNode currentlySelected => _currentlySelected;

    bool active;
    bool atTop => currentlySelected.Parent.Label == "_top";

    PlayMenuNode special1Ref => ((PlayMenuInternalNode) tree.Children[3]).Children[0];
    PlayMenuNode special2Ref => ((PlayMenuInternalNode) tree.Children[3]).Children[1];

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }

    void Start ()
    {
        GetComponent<PlayMenuFollower>().enabled = false;
    }

    void Update ()
    {
        if (!active) return;

        setSpecialNames();

        traverseMenu();
    }

    public void StartGame ()
    {
        active = true;
        GetComponent<PlayMenuFollower>().enabled = true;
    }

    void setSpecialNames ()
    {
        string name1 = "", name2 = "";

        var col = MageSquad.Instance.ActiveMage.Color;
        switch (col)
        {
            case MagicColor.Red:
                name1 = "Nimbility";
                name2 = "Bombash";
                break;

            case MagicColor.Green:
                name1 = "Recoup";
                name2 = "Rejuve";
                break;

            case MagicColor.Blue:
                name1 = "Embank";
                name2 = "???";
                break;

            default:
                throw new ArgumentException($"unexpected MagicColor {col}");
        }

        special1Ref.Label = name1;
        special2Ref.Label = name2;
    }

    void traverseMenu ()
    {
        if (Input.GetButtonDown("Play Menu Select"))
        {
            if (currentlySelected == null)
            {
                setSelected(tree.Children[0]);
            }
            else if (currentlySelected is PlayMenuLeafNode)
            {
                ((PlayMenuLeafNode) currentlySelected).OnSelect();
            }
            else if (currentlySelected is PlayMenuInternalNode)
            {
                setSelected(((PlayMenuInternalNode) currentlySelected).Children[0]);
            }
        }

        if (Input.GetButtonDown("Play Menu Back"))
        {
            if (currentlySelected != null && atTop)
            {
                setSelected(null);
            }
            else if (currentlySelected != null)
            {
                setSelected(currentlySelected.Parent);
            }
        }

        if (currentlySelected == null) return;

        if (Input.GetButtonDown("Play Menu Next"))
        {
            setSelected(currentlySelected.GetNextSibling());

            StopAllCoroutines();
            StartCoroutine(fakeCarouselAnimation(false));
        }

        if (Input.GetButtonDown("Play Menu Previous"))
        {
            setSelected(currentlySelected.GetPreviousSibling());

            StopAllCoroutines();
            StartCoroutine(fakeCarouselAnimation(true));
        }
    }

    void setSelected (PlayMenuNode selected)
    {
        _currentlySelected = selected;

        MenuEntries.text = "";

        if (selected == null) return;

        foreach (var node in selected.GetSurrounding(selected.Siblings.Count))
        {
            MenuEntries.text += node.Label + MenuEntrySeparator;
        }

        MenuEntries.text = MenuEntries.text.Substring(0, MenuEntries.text.Length - MenuEntrySeparator.Length);
    }

    IEnumerator fakeCarouselAnimation (bool fromLeft)
    {
        MenuEntries.transform.localPosition = MenuAnimationInitialOffset * Vector2.right * (fromLeft ? -1 : 1);

        Vector2 vel = Vector2.zero;

        while (MenuEntries.transform.localPosition != Vector3.zero)
        {
            MenuEntries.transform.localPosition = Vector2.SmoothDamp(MenuEntries.transform.localPosition, Vector2.zero, ref vel, MenuAnimationTime);
            yield return null;
        }
    }
}
