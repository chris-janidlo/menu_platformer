using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ColorMapApplierParticles : MonoBehaviour
{
    public MagicColor Color;
    public ParticleSystem ParticleSystem;

    void Start ()
    {
        ChangeColor(Color);

        MagicColorStats.ColorMapChanged += () => ChangeColor(Color);
    }

    public void ChangeColor (MagicColor color)
    {
        Color = color;

        var m = ParticleSystem.main;
        m.startColor = MagicColorStats.ColorMap[Color];
    }
}
