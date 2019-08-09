using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ElementalParticleEffect : MonoBehaviour
{
    public ParticleSystem ParticleSystem;

    void Update ()
    {
        transform.rotation = Quaternion.identity;
    }

    public void SetColor (MagicColor color)
    {
        var m = ParticleSystem.main;
        m.startColor = MagicColorStats.ColorMap[color];
    }
}
