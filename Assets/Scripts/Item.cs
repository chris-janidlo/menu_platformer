using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
    public bool IsHealth;
    public float Gravity;

    public ParticleSystem PickUpEffect;

    Rigidbody2D rb;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update ()
    {
        rb.velocity += Vector2.down * Gravity * Time.deltaTime;
    }

    void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.GetComponent<Mage>() != null)
        {
            if (IsHealth)
            {
                MageSquad.Instance.HealthPots++;
            }
            else
            {
                MageSquad.Instance.ManaPots++;
            }

            PickUpEffect.transform.parent = null;
            PickUpEffect.Play();
            Destroy(PickUpEffect.gameObject, PickUpEffect.main.duration);
            Destroy(gameObject);
        }
    }
}
