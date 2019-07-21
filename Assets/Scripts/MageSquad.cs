using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class MageSquad : Singleton<MageSquad>
{
    public Mage ActiveMage { get; set; }
    public Mage RedMage, GreenMage, BlueMage;

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }
}
