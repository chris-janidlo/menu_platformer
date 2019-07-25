using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class MageSquad : Singleton<MageSquad>
{
    public Mage ActiveMage, RedMage, GreenMage, BlueMage;

    public float HealthPotGain, ManaPotGain;

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

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }

    public void SetActive (MagicColor color)
    {
        switch (color)
        {
            case MagicColor.Red:
                ActiveMage = RedMage;
                break;

            case MagicColor.Green:
                ActiveMage = GreenMage;
                break;

            case MagicColor.Blue:
                ActiveMage = BlueMage;
                break;
        }
    }
}
