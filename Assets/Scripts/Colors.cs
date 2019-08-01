using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorMap
{
    public static Dictionary<MagicColor, Color> Value = new Dictionary<MagicColor, Color>
    {
        { MagicColor.Red, new Color(208 / 255f, 70 / 255f, 72 / 255f) },
        { MagicColor.Green, new Color(109 / 255f, 170 / 255f, 44 / 255f) },
        { MagicColor.Blue, new Color(89 / 255f, 124 / 255f, 206 / 255f) }
    };
}

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
    public const float SuperEffectiveDamage = 1.5f, WeakDamage = .75f;
}
