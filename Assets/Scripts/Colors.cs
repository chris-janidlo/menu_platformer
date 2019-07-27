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
