using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Teleporter : MonoBehaviour
{
    [Header("Stats")]
    public float CooldownTime;
    [Tooltip("In terms of active mage distance. Arrow is most visible when mage is at most x units away, and totally invisible when mage is at least y units away.")]
    public Vector2 ArrowVisibilityRange;

    [Header("References")]
    public Teleporter DestinationTele;
    public Transform DestinationPoint;
    public Collider2D Blocker;
    public SpriteRenderer Arrow;

    bool active;
    float cooldown;

    void Update ()
    {
        setArrowAlpha();

        cooldown -= Time.deltaTime;
        if (!active && cooldown < 0)
        {
            setActive(true);
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (!active) return;

        other.transform.position = DestinationPoint.transform.position;

        setActive(false);
        DestinationTele.setActive(false);
    }

    void setArrowAlpha ()
    {
        var dist = Vector2.Distance(Arrow.transform.position, MageSquad.Instance.ActiveMage.transform.position);

        // map dist onto 0-1 range
        dist = Mathf.Clamp(dist, ArrowVisibilityRange.x, ArrowVisibilityRange.y);
        var normalized = (dist - ArrowVisibilityRange.x) / (ArrowVisibilityRange.y - ArrowVisibilityRange.x);

        Arrow.SetAlpha(active ? (1 - normalized) : 0);
    }

    void setActive (bool value)
    {
        if (active == value) return;

        active = value;

        Blocker.enabled = !active;

        if (!active) cooldown = CooldownTime;
    }
}
