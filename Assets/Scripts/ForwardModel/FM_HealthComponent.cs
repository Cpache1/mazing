using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM_HealthComponent
{
    float health;
    float maxHealth;
    float lastHealth;
    float deltaHealth = 0;
    float regenRate = 0.05f;

    public bool playerHealing = false;

    public FM_HealthComponent(int h, int maxH)
    {
        health = h;
        maxHealth = maxH;

        lastHealth = health;
    }

    public float GetHealth() { return health; }
    public void SetHealth(int h) { health = h; } //forward model/state rep update needs this
    public float GetDeltaHealth() { return deltaHealth; }
    public void SetDeltaHealth(int dH) { deltaHealth = dH; }
    public void AddHealth(float h)
    {
        health += h;
        if (health > maxHealth) health = maxHealth;
        if (health < 0) health = 0;
    }

    public void Update(bool regen)
    {
        if (regen)
        {
            if (health > 0 && health != maxHealth)
                RegenerateHealth();
            else
                playerHealing = false;
        }

        deltaHealth = health - lastHealth;
        lastHealth = health;
    }

    private void RegenerateHealth()
    { 
        AddHealth(regenRate);
        playerHealing = true;
    }
   
}
