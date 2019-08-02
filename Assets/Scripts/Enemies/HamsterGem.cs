using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

[RequireComponent(typeof(Rigidbody2D))]
public class HamsterGem : MonoBehaviour
{
    public Vector2 LaunchSpeed;
    public float Gravity;

    public ColorMapApplier ColorPart;

    Rigidbody2D rb;
    bool launched;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
    }

    void Update ()
    {
        if (launched) rb.velocity += Vector2.down * Gravity * Time.deltaTime;
    }
    
    void OnBecameInvisible ()
    {
        if (launched) Destroy(gameObject);
    }

    public void Launch ()
    {
        transform.parent = null;

        launched = true;
        rb.simulated = true;
        rb.velocity = LaunchSpeed;
        // half the time, flip x:
        if (RandomExtra.Chance(.5f)) rb.velocity *= Vector2.left;
    }
}
