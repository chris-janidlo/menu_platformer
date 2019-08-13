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
    [Header("Stats")]
    public string MenuEntrySeparator;
    public float CarouselAnimationInitialOffset, CarouselAnimationTime;
    public float GrowAnimationTime, ShrinkAnimationTime;
    public float ShiftAnimationOffset, ShiftAnimationTime;
    public float ClickFlashFadeTime;

    [Header("References")]
    public TextMeshProUGUI MenuEntries;
    public Mask Mask;

    PlayMenuInternalNode tree = new PlayMenuInternalNode
    (
        "_top",
        new PlayMenuInternalNode
        (
            "Magic",
            new PlayMenuLeafNode
            (
                "Shoot",
                activeMage => activeMage.CastLine()
                
            ),
            new PlayMenuLeafNode
            (
                "Lob",
                activeMage => activeMage.CastLob()
                
            ),
            new PlayMenuLeafNode
            (
                "Burst",
                activeMage => activeMage.CastBurst()
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
            "Ability",
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
            "Team",
            new PlayMenuLeafNode
            (
                "next",
                () => MageSquad.Instance.SetActive(getTeamMate(true))
            ),
            new PlayMenuLeafNode
            (
                "previous",
                () => MageSquad.Instance.SetActive(getTeamMate(false))
            )
        )
    );

    // currentlySelected is duplicated as a reminder to never set it directly; instead use setSelected
    PlayMenuNode _currentlySelected;
    PlayMenuNode currentlySelected => _currentlySelected;

    PlayMenuNode lastSelected;

    bool active;
    bool atTop => currentlySelected.Parent.Label == "_top";

    PlayMenuNode special1Ref => ((PlayMenuInternalNode) tree.Children.Single(n => n.Label == "Ability")).Children[0];
    PlayMenuNode special2Ref => ((PlayMenuInternalNode) tree.Children.Single(n => n.Label == "Ability")).Children[1];

    PlayMenuNode teamNextRef => ((PlayMenuInternalNode) tree.Children.Single(n => n.Label == "Team")).Children[0];
    PlayMenuNode teamPrevRef => ((PlayMenuInternalNode) tree.Children.Single(n => n.Label == "Team")).Children[1];

    Vector2 initialMaskPosition;

    Color initialButtonColor, currentClickColor;

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }

    void Start ()
    {
        GetComponent<PlayMenuFollower>().enabled = false;

        initialMaskPosition = Mask.transform.localPosition;
        initialButtonColor = MenuEntries.color;
        currentClickColor = initialButtonColor;

        lastSelected = tree.Children[0]; // so when we first open the menu, we open it to the start of the tree
    }

    void Update ()
    {
        if (!active) return;

        if (currentlySelected != null)
        {
            setSpecialNames();
            setTeamNames();
            setText();
        }

        traverseMenu();
    }

    public void StartGame ()
    {
        StartCoroutine(startRoutine());
    }

    IEnumerator startRoutine ()
    {
        // wait one frame before activating so that if the user pressed spacebar to start the game the menu won't open up
        yield return null;

        active = true;
        GetComponent<PlayMenuFollower>().enabled = true;
    }

    static MagicColor getTeamMate (bool next)
    {
        return Instance._getTeamMate(next);
    }

    MagicColor _getTeamMate (bool next)
    {
        MagicColor col = MageSquad.Instance.ActiveMage.Color;

        switch (col)
        {
            case MagicColor.Red:
                return next ? MagicColor.Green : MagicColor.Blue;
            
            case MagicColor.Green:
                return next ? MagicColor.Blue : MagicColor.Red;

            case MagicColor.Blue:
                return next ? MagicColor.Red : MagicColor.Green;

            default:
                throw new Exception($"unexpected active mage color {col}");
        }
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
                name1 = "Swap";
                name2 = "Stop";
                break;

            default:
                throw new ArgumentException($"unexpected MagicColor {col}");
        }

        special1Ref.Label = name1;
        special2Ref.Label = name2;
    }

    void setTeamNames ()
    {
        string name1 = "", name2 = "";

        var col = MageSquad.Instance.ActiveMage.Color;
        switch (col)
        {
            case MagicColor.Red:
                name1 = "Green Mage";
                name2 = "Blue Mage";
                break;

            case MagicColor.Green:
                name1 = "Blue Mage";
                name2 = "Red Mage";
                break;

            case MagicColor.Blue:
                name1 = "Red Mage";
                name2 = "Green Mage";
                break;

            default:
                throw new ArgumentException($"unexpected MagicColor {col}");
        }

        teamNextRef.Label = name1;
        teamPrevRef.Label = name2;
    }

    void traverseMenu ()
    {
        if (Input.GetButtonDown("Play Menu Quick Toggle"))
        {
            bool shouldOpen = currentlySelected == null;
            startAnimation("sizeAnimation", shouldOpen);
            setSelected(shouldOpen ? lastSelected : null);
        }

        if (Input.GetButtonDown("Play Menu Select"))
        {
            if (currentlySelected is PlayMenuLeafNode)
            {
                if (!currentlySelected.Parent.Label.Equals("Team")) startAnimation("clickAnimation");
                ((PlayMenuLeafNode) currentlySelected).OnSelect();
            }
            else
            {
                if (currentlySelected == null)
                {
                    startAnimation("sizeAnimation", true);
                    setSelected(lastSelected.Root);
                }
                else // when currentlySelected is PlayMenuInternalNode
                {
                    startAnimation("shiftAnimation", true);
                    setSelected(((PlayMenuInternalNode) currentlySelected).Children[0]);
                }
            }
        }

        if (currentlySelected == null) return;

        if (Input.GetButtonDown("Play Menu Back"))
        {
            startAnimation(atTop ? "sizeAnimation" : "shiftAnimation", false);
            setSelected(atTop ? null : currentlySelected.Parent);
        }

        if (Input.GetButtonDown("Play Menu Next"))
        {
            startAnimation("fakeCarouselAnimation", true);
            setSelected(currentlySelected.GetNextSibling());
        }

        if (Input.GetButtonDown("Play Menu Previous"))
        {
            startAnimation("fakeCarouselAnimation", false);
            setSelected(currentlySelected.GetPreviousSibling());
        }
    }

    void setSelected (PlayMenuNode selected)
    {
        _currentlySelected = selected;

        if (selected == null) return;

        lastSelected = selected;

        setText();
    }

    void setText ()
    {
        MenuEntries.text = String.Join
        (
            MenuEntrySeparator,
            currentlySelected.GetSurrounding().Select(s => {
                if (s.Label.Equals(currentlySelected.Label))
                {
                    return $"<#{ColorUtility.ToHtmlStringRGB(currentClickColor)}>{currentlySelected.Label}</color>";
                }
                else
                {
                    return s.Label;
                }
            })
        );
    }

    void startAnimation (string animName, bool? value = null)
    {
        StopCoroutine(animName);

        if (value == null)
        {
            StartCoroutine(animName);
        }
        else
        {
            StartCoroutine(animName, value);
        }
    }

    IEnumerator clickAnimation ()
    {
        float timer = 0;
        while (timer < ClickFlashFadeTime)
        {
            currentClickColor = Color.Lerp(MageSquad.Instance.CurrentColorValue, initialButtonColor, timer / ClickFlashFadeTime);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        currentClickColor = initialButtonColor;
    }

    IEnumerator sizeAnimation (bool growing)
    {
        transform.localScale = growing ? Vector3.zero : Vector3.one;
        Vector3 target = growing ? Vector3.one : Vector3.zero;
        Vector3 vel = Vector3.zero;

        float smoothTime = growing ? GrowAnimationTime : ShrinkAnimationTime;

        while (transform.localScale != target)
        {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, target, ref vel, smoothTime * Time.timeScale);
            yield return null;
        }

        transform.localScale = (currentlySelected == null) ? Vector3.zero : Vector3.one;
    }

    IEnumerator shiftAnimation (bool goingUp)
    {
        Mask.transform.localPosition = initialMaskPosition + ShiftAnimationOffset * (goingUp ? Vector2.down : Vector2.up);

        Vector2 vel = Vector2.zero;

        while (Mask.transform.localPosition != (Vector3) initialMaskPosition)
        {
            Mask.transform.localPosition = Vector2.SmoothDamp(Mask.transform.localPosition, initialMaskPosition, ref vel, ShiftAnimationTime * Time.timeScale);
            yield return null;
        }

        Mask.transform.localPosition = initialMaskPosition;
    }

    IEnumerator fakeCarouselAnimation (bool rightToLeft)
    {
        MenuEntries.transform.localPosition = CarouselAnimationInitialOffset * Vector2.right * (rightToLeft ? 1 : -1);

        Vector2 vel = Vector2.zero;

        while (MenuEntries.transform.localPosition != Vector3.zero)
        {
            MenuEntries.transform.localPosition = Vector2.SmoothDamp(MenuEntries.transform.localPosition, Vector2.zero, ref vel, CarouselAnimationTime * Time.timeScale);
            yield return null;
        }

        MenuEntries.transform.localPosition = Vector2.zero;
    }
}
