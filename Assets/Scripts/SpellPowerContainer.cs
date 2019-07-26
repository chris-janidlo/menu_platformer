using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpellPowerContainer
{
    public float this[SpellPower power]
    {
        get
        {
            switch (power)
            {
                case SpellPower.Light:
                    return Light;

                case SpellPower.Normal:
                    return Normal;
                
                case SpellPower.Heavy:
                    return Heavy;
                
                default:
                    throw new ArgumentException($"unexpected enum value {power}");
            }
        }
    }

    public float Light, Normal, Heavy;
}
