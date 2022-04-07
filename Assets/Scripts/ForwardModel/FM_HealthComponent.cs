using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM_HealthComponent
{
    int health;
    int maxHealth;

    public FM_HealthComponent(int h, int maxH)
    {
        health = h;
        maxHealth = maxH;
    }

    public int GetHealth() { return health; }
    public void AddHealth(int h)
    {
        health += h;
        if (health > maxHealth) health = maxHealth;
        if (health < 0) health = 0;
    }
   
}
