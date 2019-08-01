using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class HamsterFartCloud : MonoBehaviour
{
    public Vector2 LifeTimeRange;
    [Range(0, 1)]
    public float Alpha;
    public ColorMapApplier ColorPart;
    public SpriteRenderer Outline, Cloud;
    public Sprite Outline1, Cloud1, Outline2, Cloud2;

    HamsterFart parent;

    public void Initialize (HamsterFart parent, Vector2 velocity, MagicColor color)
    {
        this.parent = parent;

        ColorPart.ChangeColor(color);

        bool use1 = RandomExtra.Chance(.5f);
        Outline.sprite = use1 ? Outline1 : Outline2;
        Cloud.sprite = use1 ? Cloud1 : Cloud2;

        Outline.SetAlpha(Alpha);
        Cloud.SetAlpha(Alpha);

        GetComponent<Collider2D>().isTrigger = true; // just in case
        GetComponent<Rigidbody2D>().velocity = velocity;

        Destroy(gameObject, RandomExtra.Range(LifeTimeRange));
    }

    // set it so that as long as a mage is touching at least one cloud, it will take damage

    void OnTriggerEnter2D (Collider2D other)
    {
        parent.SetMageStatus(other.GetComponent<Mage>(), true);
    }

    void OnTriggerStay2D (Collider2D other)
    {
        parent.SetMageStatus(other.GetComponent<Mage>(), true);
    }

    void OnTriggerExit2D (Collider2D other)
    {
        parent.SetMageStatus(other.GetComponent<Mage>(), false);
    }
}
