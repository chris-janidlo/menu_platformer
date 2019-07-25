using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class MageSquad : Singleton<MageSquad>
{
    public Mage ActiveMage, RedMage, GreenMage, BlueMage;

    public float HealthPotGain, ManaPotGain;

    public int HealthPots, ManaPots;

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
