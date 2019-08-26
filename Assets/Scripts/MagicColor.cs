using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MagicColor
{
    Red, Green, Blue
}

public static class MagicColorExtensions
{
    // uses c# compare convention to define greater-than relation. semantically, if x > y, then x is super effective against y and y is weak against x; if x == y, then x and y have neutral effects with one another
    public static int Compare (this MagicColor x, MagicColor y)
    {
        // from https://stackoverflow.com/a/9553712/5931898
        int d = (3 + y - x) % 3;

        if (d == 2) return -1;
        else return d;
    }
}

public static class MagicColorStats
{
    public static event Action ColorMapChanged;
 
    public const float SuperEffectiveDamage = 1.5f, WeakDamage = .75f;

    public static Dictionary<MagicColor, Color> ColorMap
    {
        get
        {
            if (PlayerPrefs.HasKey("ColorBlindMode") && ColorBlindMode)
            {
                return ColorBlindMap;
            }
            else
            {
                return DefaultColorMap;
            }
        }
    }

    public static bool ColorBlindMode
    {
        get => PlayerPrefs.GetInt("ColorBlindMode") == 1;
        set
        {
            PlayerPrefs.SetInt("ColorBlindMode", value ? 1 : 0); 

            if (ColorMapChanged != null) ColorMapChanged();
        }
    }

    public static readonly Dictionary<MagicColor, Color> DefaultColorMap = new Dictionary<MagicColor, Color>
    {
        { MagicColor.Red, new Color(208 / 255f, 70 / 255f, 72 / 255f) },
        { MagicColor.Green, new Color(109 / 255f, 170 / 255f, 44 / 255f) },
        { MagicColor.Blue, new Color(89 / 255f, 124 / 255f, 206 / 255f) }
    };

    // changes green to yellow
    public static readonly Dictionary<MagicColor, Color> ColorBlindMap = new Dictionary<MagicColor, Color>
    {
        { MagicColor.Red, new Color(208 / 255f, 70 / 255f, 72 / 255f) },
        { MagicColor.Green, new Color(218 / 255f, 212 / 255f, 94 / 255f) },
        { MagicColor.Blue, new Color(89 / 255f, 124 / 255f, 206 / 255f) }
    };
}
