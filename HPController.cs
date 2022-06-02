using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPController : MonoBehaviour, IDamgeable
{
    public float myStartingHealth = 100.0f;
    public float health;
    protected bool isHitted;
    public bool dead;
    public float dealtDamage;

    protected virtual void Start()
    {
        health = myStartingHealth;
    }
    public virtual void TakeHit(float damage)
    {
        isHitted = true;
        health -= damage;
        dealtDamage = Mathf.Round(damage *10) * 0.1f;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        dead = true;
        GameObject.Destroy(gameObject);
    }
}
