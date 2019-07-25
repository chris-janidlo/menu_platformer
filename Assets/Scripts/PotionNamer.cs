using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PotionNamer : MonoBehaviour
{
    public bool IsHealth;
    public string Prefix;

    TextMeshProUGUI text;

    void Start ()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update ()
    {
        var val = IsHealth ? MageSquad.Instance.HealthPots : MageSquad.Instance.ManaPots;
        text.text = Prefix + val.ToString();
    }
}
