using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Note that if you change values in game these must be changed as well.
public class FM_FrustrationComponent
{
    private bool frustrationIsActive;

    private int minFrustration = 0;
    private int maxFrustration = 100;
    private float levelOfFrustration;

    public FM_FrustrationComponent()
    {
        levelOfFrustration = minFrustration;
    }

    public void SetFrustration(float f) { levelOfFrustration = f; }
    public float GetFrustration() { return levelOfFrustration; }
    public void SetActive(bool mode) { frustrationIsActive = mode; }

    public void Update()
    {
        // Keeps frustration 0 (off) for the session
        if (!frustrationIsActive)
        {
            levelOfFrustration = minFrustration;
        }

        // Correcting min-max setup of frustration
        ClampFrustration();
    }

    private void ClampFrustration()
    {
        if (maxFrustration < minFrustration)
        {
            maxFrustration = minFrustration;
        }
        if (levelOfFrustration < minFrustration)
        {
            levelOfFrustration = minFrustration;
        }
        if (levelOfFrustration > maxFrustration)
        {
            levelOfFrustration = maxFrustration;
        }
    }

}
