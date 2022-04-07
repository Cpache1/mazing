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
    public bool shoot;

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
    private float rotationSpeed;


    public FM_GunControlComponent()
    {
        SetupGunControls();
    }

    private void SetupGunControls()
    {
        shoot = false;
        startingBullets = 5;

        bulletSpeed = 1000;
        rotationSpeed = 0.0f;
    }

    public void Update(FM_Player p, FM_Game game)
    {
        if (shoot)
        {
            shoot = false;
            Shoot(p, game);
        }
    }

    public void Shoot(FM_Player _player, FM_Game _game)
    {
        //get positions and velocities/direction
        Vector2 compDir = _player.GetMovementComponent().GetDir();
        Vector2 pos = _player.GetPosition() + compDir;

        //find a "dead" bullet and revive it
        FM_GameObject[] projectiles = _game.GetGameObjects();
        for (int i = 0; i < startingBullets; i++)
        {
            if(!projectiles[i].IsAlive())
            {
                projectiles[i].SetPosition(pos);
                projectiles[i].GetMovementComponent().SetVel(compDir.x, compDir.y);
                projectiles[i].Revive();
                break;
            }
        }
    }
}
