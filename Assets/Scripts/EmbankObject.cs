using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EmbankObject : MonoBehaviour
{
    public float Gravity;

    Rigidbody2D rb;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update ()
    {
        rb.velocity += Vector2.down * Gravity * Time.deltaTime;
    }
}
