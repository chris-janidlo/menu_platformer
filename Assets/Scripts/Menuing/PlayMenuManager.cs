using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using crass;

public class PlayMenuManager : Singleton<PlayMenuManager>
{
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

        traverseMenu();
    }

    public void StartGame ()
    {
        active = true;
        GetComponent<PlayMenuFollower>().enabled = true;

        setSelected(tree.Children[0]);
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
            if (atTop)
            {
                setSelected(null);
            }
            else
            {
                setSelected(currentlySelected.Parent);
            }
        }

        if (currentlySelected == null) return;

        if (Input.GetButtonDown("Play Menu Next"))
        {
            setSelected(currentlySelected.GetNextSibling());
        }

        if (Input.GetButtonDown("Play Menu Previous"))
        {
            setSelected(currentlySelected.GetPreviousSibling());
        }
    }

    void setSelected (PlayMenuNode selected)
    {
        Debug.Log(selected?.Label);
        _currentlySelected = selected;
    }
}
