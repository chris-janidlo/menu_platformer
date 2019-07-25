using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Mage : MonoBehaviour
{
    public const float MaxHealth = 100, MaxMana = 100;

    public MagicColor Color => Visuals.Color;

    [SerializeField]
    float _health = MaxHealth, _mana = MaxMana;

    public float Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
            if (_health == 0) die();
        }
    }

    public float Mana
    {
        get => _mana;
        set => _mana = Mathf.Clamp(value, 0, MaxMana);
    }

    public ColorMapApplier Visuals;

    public float ManaGain;
    public List<float> BurstCosts, LineCosts, LobCosts;

    public float MoveSpeed;
    public float AirAcceleration;
    public float JumpSpeedBurst, JumpSpeedCut;
    public float Gravity;
    public float JumpFudgeTime;
    public float GroundedFudgeVertical = 0.1f, GroundedFudgeHorizontal = 0.1f;

    Rigidbody2D rb;
    float halfHeight;
    Vector2 groundedExtents;
    float timeSinceLastJumpPress = float.MaxValue;

    RaycastHit2D[] groundedResults;
    ContactFilter2D groundedFilter;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();

        var extents = GetComponent<Collider2D>().bounds.extents;
        halfHeight = extents.y;
        groundedExtents = new Vector2
        (
            extents.x + GroundedFudgeHorizontal / 2,
            GroundedFudgeVertical / 2
        );

        groundedFilter = new ContactFilter2D();
        groundedFilter.NoFilter();
    }

    void Update ()
    {
        platform();

        Mana += ManaGain * Time.deltaTime;
    }

    // returns whether the cast was successful
    public bool CastBurst (SpellPower power)
    {
        throw new NotImplementedException();
    }

    // returns whether the cast was successful
    public bool CastLine (SpellPower power)
    {
        throw new NotImplementedException();
    }

    // returns whether the cast was successful
    public bool CastLob (SpellPower power)
    {
        throw new NotImplementedException();
    }

    public void LongJump ()
    {
        throw new NotImplementedException();
    }

    public void HighJump ()
    {
        throw new NotImplementedException();
    }

    // returns whether the cast was successful
    public bool Special1 ()
    {
        throw new NotImplementedException();
    }

    // returns whether the cast was successful
    public bool Special2 ()
    {
        throw new NotImplementedException();
    }

    // returns true if there was at least one potion at method call
    public bool DrinkHealthPotion ()
    {
        if (MageSquad.Instance.HealthPots == 0) return false;

        MageSquad.Instance.HealthPots--;
        Health += MageSquad.Instance.HealthPotGain;

        return true;
    }

    // returns true if there was at least one potion at method call
    public bool DrinkManaPotion ()
    {
        if (MageSquad.Instance.ManaPots == 0) return false;

        MageSquad.Instance.ManaPots--;
        Health += MageSquad.Instance.ManaPotGain;

        return true;
    }

    void die ()
    {
        throw new NotImplementedException();
    }

    void platform ()
    {
        bool active = (MageSquad.Instance.ActiveMage == this);

        float move = active ? Input.GetAxisRaw("Move") : 0;
        bool jumpHold = active ? Input.GetButton("Jump") : false;
        if (active && Input.GetButtonDown("Jump"))
        {
            timeSinceLastJumpPress = 0;
        }
        
        if (isGrounded())
        {
            rb.velocity = Vector2.right * MoveSpeed * move;

            if (timeSinceLastJumpPress <= JumpFudgeTime)
            {
                rb.velocity = new Vector3(rb.velocity.x, JumpSpeedBurst);
            }
        }
        else
        {
            var newX = rb.velocity.x + move * AirAcceleration * Time.deltaTime;
            newX = Mathf.Clamp(newX, -MoveSpeed, MoveSpeed);

            var newY = rb.velocity.y - Gravity * Time.deltaTime;

            if (!jumpHold && newY > JumpSpeedCut)
            {
                newY = JumpSpeedCut;
            }

            rb.velocity = new Vector2(newX, newY);
        }

        timeSinceLastJumpPress += Time.deltaTime;
    }

    bool isGrounded ()
    {
        groundedResults = new RaycastHit2D[2]; // don't need more than two; one for us, one for the ground
        Physics2D.BoxCast(transform.position, groundedExtents, 0, Vector2.down, groundedFilter, groundedResults, halfHeight);

        foreach (var result in groundedResults)
        {
            if (result.collider != null && result.collider.gameObject != gameObject)
            {
                return true;
            }
        }

        return false;
    }
}

public enum SpellPower
{
    Normal, Heavy, Light
}
