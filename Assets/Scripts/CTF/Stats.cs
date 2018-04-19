using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stats are capture the flag object's health and knockback
public abstract class Stats : CTFObject {
    public int health = 3;
    public float knockback = 30f;

    // Hurt the object and knock it in the given direction
    public abstract void Damage(Vector3 dir);

    // Increment the object's health
    public void Heal()
    {
        health++;
    }

    // Handle the object losing all health
    public abstract void Die();
}
