using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM_HealthComponent
{
    int health;
    int maxHealth;
    int lastHealth;
    int deltaHealth = 0;

    public FM_HealthComponent(int h, int maxH)
    {
        health = h;
        maxHealth = maxH;

        lastHealth = health;
    }

    public int GetHealth() { return health; }
    public void SetHealth(int h) { health = h; } //forward model/state rep update needs this
    public int GetDeltaHealth() { return deltaHealth; }
    public void SetDeltaHealth(int dH) { deltaHealth = dH; }
    public void AddHealth(int h)
    {
        health += h;
        if (health > maxHealth) health = maxHealth;
        if (health < 0) health = 0;
    }

    public void Update()
    {
        deltaHealth = health - lastHealth;
        lastHealth = health;
    }
   
}
