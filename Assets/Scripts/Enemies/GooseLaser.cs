using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(DestroyWhenChildrenInvisible))]
public class GooseLaser : MonoBehaviour
{
    public float Speed, Damage;

    public void Initialize (Vector2 direction)
    {
        GetComponent<Rigidbody2D>().velocity = direction * Speed;
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        other.GetComponent<ColoredHealth>().PureDamage(Damage);
        Destroy(gameObject);
    }
}
