﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Monte;

public class FM_Game
{
    Time elapsed;
    private int idleTime;
    public int fullTime;
    private Vector2 previousPlayerPosition;
    private Vector2 previousBotPosition;

    private int score;

    public AIState state;
    public float[] stateRep;
    public FM_Player player;
    public FM_Monster monster;
    public FM_Grid grid;

    FM_GameObject[] gameObjects;
    List<ProjectileStruct> structures;
    int startProjectileIndex = 2; //in game objects array

    public FM_Game(FM_Grid g, FM_GameObject[] gameObjs)
    {
        grid = g;

        gameObjects = gameObjs;
        structures = new List<ProjectileStruct>();
        player = (FM_Player)gameObjects[0]; //player is first
        monster = (FM_Monster)gameObjects[1]; //monster second

        //state = currentState;
    }

    public void AddScore(int s) 
    { 
        score += s;
        if (score < 0) 
            score = 0;
    }


    //action[0] = x, horizontal input
    //action[1] = y, vertical input

    public float[] Apply(float[] action, AIState providedState)
    {
        if (providedState.playerIndex == 0) //player has more things it can do
        {
            player.GetMovementComponent().SetVel(action[0], action[1]);
        }
        else if (providedState.playerIndex == 1)
        {
            monster.GetMovementComponent().SetVel(action[0], action[1]);
        }

        return providedState.stateRep;
    }

    //TESTING WITH FM_VISUALTEST
    public void Apply(float[] action, FM_GameObject agent)
    {
        if (agent.GetType() == FM_GameObjectType.Player) //player has more things it can do
        {
            player.GetMovementComponent().SetVel(action[0], action[1]);
        }
        else if (agent.GetType() == FM_GameObjectType.Monster)
        {
            monster.GetMovementComponent().SetVel(action[0], action[1]);
        }

    }

    public void GiveInputs(bool shooting, bool bomb)
    {
        if (shooting)
        {
            shooting = false;
            player.GetGunControl().shoot = true;
        }
    }


    public void CreateGameObjectAt(Vector2 pos)
    {
        
    }

    /*public void AddGameObject(FM_GameObject obj)
    {
        gameObjects.Add(obj);
    }*/
    public FM_GameObject[] GetGameObjects() { return gameObjects; }

    public void HandleInput()
    {
        //checks what's being pressed and if something is being pressed
        //player "handle" the input
    }

    public void UpdateGame()
    {
        if (player.GetMovementComponent().GetVel()==new Vector2(0.0f, 0.0f) &&
            !player.GetGunControl().shoot)
        {
            //Starts counting when no button is being pressed
            idleTime = idleTime + 1;
        }
        fullTime = fullTime + 1;

        //Updates all entities        
        foreach (FM_GameObject obj in gameObjects)
        {
            if (obj.IsAlive())
            {
                obj.Update(this, 1.0f);
            }
        }


        //COLLISIONS (between moving objects)
        if (player.IsAlive() && monster.IsAlive()) //if they're not, game is over so no point checking collisions
        {
            //player and monster first
            if (player.IntersectsWith(monster))
            {
                player.OnCollisionEnter(monster, this);
            }
            //bullets, bombs and walls after
            for (int j = startProjectileIndex; j < gameObjects.Length; j++)
            {
                if (gameObjects[j].IsAlive()) //if not alive don't bother
                {
                    //intersecting with player
                    if (gameObjects[j].IntersectsWith(player))
                    {
                        gameObjects[j].OnCollisionEnter(player, this);
                    }
                    //intersecting with monster (remember bombs can intersect both at the same time)
                    if (gameObjects[j].IntersectsWith(monster))
                    {
                        gameObjects[j].OnCollisionEnter(monster, this);
                    }
                }
            }
        }
        else if (!player.IsAlive())
        {
            AddScore(-25);
        }
        else if (!monster.IsAlive())
        {
            AddScore(15);
        }

    }

    //Update game based on the new state representation given by FM
    public void UpdateStateRep(float[] currentState, List<ProjectileStruct> projectiles)
    {
        stateRep = currentState;

        //time and score
        idleTime = (int)stateRep[0];
        score = (int)stateRep[1];

        //monster
        previousBotPosition = new Vector2(stateRep[3], stateRep[4]);        
        monster.SetPosition(new Vector2(stateRep[3], stateRep[4]));
        float degrees = stateRep[5];
        monster.GetMovementComponent().SetDir((float)Math.Cos(degrees * Math.PI / 180), (float)Math.Sin(degrees * Math.PI / 180)); // degrees to vector
        monster.GetMovementComponent().SetSpeed(stateRep[6]);
        monster.GetMovementComponent().SetRotationSpeed(stateRep[7]);

        monster.GetHealthComponent().SetHealth((int)stateRep[14]);
        monster.GetHealthComponent().SetDeltaHealth((int)stateRep[47]); //botDeltaHealth
        if (stateRep[48] == 0)
            monster.Revive();
        else
            monster.DeleteGameObject();

        monster.GetFrustrationComponent().SetFrustration(stateRep[15]);
        monster.GetFrustrationComponent().SetActive(true); //TODO: When do we turn it off?

        //player
        previousPlayerPosition = new Vector2(stateRep[24], stateRep[25]);
        player.SetPosition(new Vector2(stateRep[24], stateRep[25]));
        degrees = stateRep[26];
        player.GetMovementComponent().SetDir((float)Math.Cos(degrees * Math.PI / 180), (float)Math.Sin(degrees * Math.PI / 180));

        player.GetHealthComponent().SetHealth((int)stateRep[27]);
        player.GetHealthComponent().SetDeltaHealth((int)stateRep[42]);
        if (stateRep[43] == 0)
            player.Revive();
        else
            player.DeleteGameObject();

        //monster/player related
        
        

        //bullets + bombs
        int projectileIdx = 0; //in list

        int noBullets = (int)stateRep[50]; //check how many bullets are alive 
        for (int i = startProjectileIndex; i < gameObjects.Length - 3; i++)
        {

            if (projectiles[projectileIdx].alive)
            {
                gameObjects[i].SetPosition(new Vector2(projectiles[projectileIdx].x, projectiles[projectileIdx].y));
                gameObjects[i].GetMovementComponent().SetVel(projectiles[projectileIdx].dirX, projectiles[projectileIdx].dirY);
                gameObjects[i].Revive();
                projectileIdx++;
                noBullets--;
            }
            else
            {
                gameObjects[i].DeleteGameObject();
            }
        }

        int noFires = (int)stateRep[49]; //check how many bombs are ignited 
        for (int i = gameObjects.Length - 3; i < gameObjects.Length; i++)
        {
            if (noFires!=0)
            {
                /*gameObjects[i].SetPosition(new Vector2(x, y));
                gameObjects[i].GetMovementComponent().SetVel(compDir.x, compDir.y); //if not ignited
                //gameObjects[i].GetMovementComponent().SetDir((float)Math.Cos(degrees * Math.PI / 180), (float)Math.Sin(degrees * Math.PI / 180));
                //SET TIME TO LIVE IF IGNITED
                gameObjects[i].Revive();
                projectileIdx++;
                noFires--;*/
            }
            else
            {
                gameObjects[i].DeleteGameObject();
            }
        }

        
        
    }

    //Gets the current state based on the state rep
    //Call this only after UpdateGame().
    //TODO: the rotation might be wrong because I am considering origin looking at right
    //the game itself might make it looking at left...
    //return the new state after updates
    public float[] GetNextState()
    {
        //game has been updated, update state representation
        stateRep[0] = ((float)idleTime / (float)fullTime); //idleTime
        stateRep[55] = 1 - ((float)idleTime / (float)fullTime); //[general]activity
        stateRep[1] = score; //score
        stateRep[56] = score; //[general]score

        Vector2 origin = new Vector2(0.0f, 0.0f);

        //monster
        stateRep[2] = (monster.GetPosition() - previousBotPosition).magnitude; //botDistanceTraveled
        stateRep[3] = monster.GetPosition().x;
        stateRep[4] = monster.GetPosition().y;
        stateRep[5] = Vector2.SignedAngle(origin, monster.GetMovementComponent().GetDir());
        stateRep[6] = monster.GetMovementComponent().GetSpeed();
        stateRep[7] = monster.GetMovementComponent().GetRotationSpeed();
        stateRep[14] = monster.GetHealthComponent().GetHealth();
        stateRep[47] = monster.GetHealthComponent().GetDeltaHealth();
        stateRep[48] = monster.IsAlive() ? 0 : 1; //botDied
        stateRep[15] = monster.GetFrustrationComponent().GetFrustration();


        //player
        stateRep[23] = (player.GetPosition() - previousPlayerPosition).magnitude;//playerDistanceTravelled
        stateRep[24] = player.GetPosition().x;
        stateRep[25] = player.GetPosition().y;
        stateRep[26] = Vector2.SignedAngle(origin, player.GetMovementComponent().GetDir());
        stateRep[27] = player.GetHealthComponent().GetHealth();
        stateRep[42] = player.GetHealthComponent().GetDeltaHealth();
        stateRep[43] = player.IsAlive() ? 0 : 1; //playerDied

        //player/monster
        stateRep[20] = (player.GetPosition() - monster.GetPosition()).magnitude;

        //bullets + bombs
        int noBullets = 0;
        int noFires = 0;
        structures.Clear();
        for (int i = startProjectileIndex; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].GetType()==FM_GameObjectType.Bullet)
            {
                ProjectileStruct bullet = new ProjectileStruct();
                bullet.x = gameObjects[i].GetPosition().x;
                bullet.y = gameObjects[i].GetPosition().y;
                bullet.dirX = gameObjects[i].GetMovementComponent().GetVel().x; 
                bullet.dirY = gameObjects[i].GetMovementComponent().GetVel().y;
                bullet.alive = gameObjects[i].IsAlive();
                structures.Add(bullet);

                if(gameObjects[i].IsAlive())
                    noBullets++;
            }
            else if (gameObjects[i].GetType() == FM_GameObjectType.Bomb && gameObjects[i].IsAlive()/*TODO: AND it is in fire state*/)
            {
                noFires++;
            }
        }
        stateRep[49] = noFires;
        stateRep[50] = noBullets;

        //cursor TODO: currently you are only counting with the option of cursor being "AI" not a human player.
        stateRep[31] = stateRep[2]; //cursorDistanceTraveled
        stateRep[32] = stateRep[3]; //cursorPositionX
        stateRep[33] = stateRep[4]; //cursorPositionY

        return stateRep;
    }

    public List<ProjectileStruct> GetProjectileStructs() { return structures; }
}


//we must keep track of where the bullets and bombs are as these are not fully part of staterep
//only their existence.
public struct ProjectileStruct
{
    public float x, y;
    public float dirX, dirY;
    public float ttl;
    public bool alive;

    public ProjectileStruct GetCopy()
    {
        ProjectileStruct p;
        p.x = this.x;
        p.y = this.y;
        p.dirX = this.dirX;
        p.dirY = this.dirY;
        p.ttl = this.ttl;
        p.alive = this.alive;
        return p;
    }
}
