using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using crass;

public class MageSquad : Singleton<MageSquad>, IEnumerable<Mage>
{
    public Mage ActiveMage, RedMage, GreenMage, BlueMage;

    public float StartMenuFollowDelay, StartMenuFollowSpacing;

    public float HealthPotGain, ManaPotGain;

    public ActiveMageChangeEvent ActiveMageChanged;

    [SerializeField]
    int _healthPots, _manaPots;

    public int HealthPots
    {
        get => _healthPots;
        set => _healthPots = Mathf.Clamp(value, 0, 99);
    }

    public int ManaPots
    {
        get => _manaPots;
        set => _manaPots = Mathf.Clamp(value, 0, 99);
    }

    public Mage this[MagicColor color]
    {
        get
        {
            switch (color)
            {
                case MagicColor.Red:
                    return RedMage;

                case MagicColor.Green:
                    return GreenMage;

                case MagicColor.Blue:
                    return BlueMage;

                default:
                    throw new ArgumentException($"unexpected MagicColor {color}");
            }
        }
    }

    bool startMenu = true;
    Vector2 redVel, greenVel, blueVel;

    void Awake ()
    {
        SingletonSetInstance(this, true);
        ActiveMage = RedMage;
    }

    void Update ()
    {
        mageStartMenuCursor();
    }

    public void StartGame ()
    {
        startMenu = false;

        RedMage.StartGame();
        GreenMage.StartGame();
        BlueMage.StartGame();
    }

    public void SetActive (MagicColor color)
    {
        var mage = this[color];

        if (mage == ActiveMage)
        {
            CantDoThatFeedback.Instance.DisplayMessage($"{color} Mage is already active!");
        }

        if (!mage.Health.Dead)
        {
            ActiveMageChanged.Invoke(ActiveMage, mage);
            ActiveMage = mage;
        }
        else
        {
            CantDoThatFeedback.Instance.DisplayMessage($"{color} Mage is dead!");
        }
    }

	public IEnumerator<Mage> GetEnumerator()
	{
        yield return RedMage;
        yield return GreenMage;
        yield return BlueMage;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
        return this.GetEnumerator();
	}

    void mageStartMenuCursor ()
    {
        if (!startMenu) return;

        RedMage.transform.position = Vector2.SmoothDamp
        (
            RedMage.transform.position,
            StartMenuSelectionFollower.Instance.transform.position,
            ref redVel,
            StartMenuFollowDelay
        );
        
        GreenMage.transform.position = Vector2.SmoothDamp
        (
            GreenMage.transform.position,
            RedMage.transform.position + Vector3.left * StartMenuFollowSpacing,
            ref greenVel,
            StartMenuFollowDelay
        );

        BlueMage.transform.position = Vector2.SmoothDamp
        (
            BlueMage.transform.position,
            GreenMage.transform.position + Vector3.left * StartMenuFollowSpacing,
            ref blueVel,
            StartMenuFollowDelay
        );
    }
}

[Serializable]
// oldMage, newMage
public class ActiveMageChangeEvent : UnityEvent<Mage, Mage> {}
