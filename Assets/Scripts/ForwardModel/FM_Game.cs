﻿//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monte;

public class FM_Game
{
    Time elapsed;

    public AIState state;
    public FM_Player player;
    public FM_Monster monster;
    public FM_Grid grid;

    FM_GameObject[] gameObjects;

    //we must keep track of where the bullets and bombs are as these are not fully part of staterep
    //only their existence.

    public FM_Game(FM_Player p, FM_Monster m, FM_Grid g, FM_GameObject[] gameObjs/*, AIState currentState*/)
    {
        player = p;
        monster = m;

        grid = g;
        gameObjects = gameObjs;
        
        //state = currentState;
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
        //Updates all entities
        player.Update(this, 1); //it would be better to have all in one list/array
        monster.Update(this, 1);
        foreach (FM_GameObject obj in gameObjects)
        {
            obj.Update(this, 1.0f);
        }


        //COLLISIONS (between moving objects)
        //player and monster first
        if (player.IntersectsWith(monster))
        {
            player.OnCollisionEnter(monster);
        }
        //bullets, bombs and walls after
        for (int j = 0; j < gameObjects.Length; j++)
        {
            if (gameObjects[j].IsAlive()) //if not alive don't bother
            {
                //intersecting with player
                if(gameObjects[j].IntersectsWith(player))
                {
                    gameObjects[j].OnCollisionEnter(player);
                }
                //intersecting with monster (remember bombs can intersect both at the same time)
                if(gameObjects[j].IntersectsWith(monster))
                {
                    gameObjects[j].OnCollisionEnter(monster);
                }
            }
        }
    }

    //public FM_GameObject GetEntity to get an entity with a specific index.
}
