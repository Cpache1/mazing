using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM_GunControlComponent
{
    //ReportGenerator reportGenerator;

    //Position when firing
    Vector3 firePosition;

    //External components
    private FM_Player player;
    //public GameObject projectile;
    //public GameObject bomb;
    //public GameObject bombTrigger; //place where the bomb lands?

    //Global variables across weapons
    public float globalCoolDown = 5f;
    private float globalTimeStamp;
    bool onGlobalCooldown = false;
    private float lastFired;

    //Public variables for the primary weapon
    public string primaryWeapon = "sixgun"; //Player starts with sixgun
    public int startingBullets;// = 6;
    public int bulletSpeed;// = 500;
    public float firingRate;// = 0.5f;
    public float reloadTime;// = 2.5f;

    //Private variables for the primary weapon cooldown
    public bool reloading = false;
    public int projectileCount;
    private float reloadTimeStamp;
    private float projectileTimeStamp;

    //Public variables for the secondary weapon
    public int startingBombs;// = 2;
    public int bombThrowSpeed;// = 250;
    public float bombThrowRate;// = 1f;
    public float bombCooldown;// = 3f;
    public int bombCount;
    //Private variables for the secondary weapon cooldown
    public bool bombReloading = false;
    private float bombCoodownTimeStamp;
    private float bombThrowTimeStamp;

    //Private variables for aiming
    //private Vector3 mousePosition;
    //private float rotationSpeed;


    public FM_GunControlComponent()
    {
        SetupGunControls();
    }

    private void SetupGunControls()
    {

    }

}
