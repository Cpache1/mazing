using System;
using System.Collections.Generic;
using UnityEngine;
using Monte;

public class FM_Game
{
    Time elapsed;

    public AIState state;
    public float[] stateRep;
    public FM_Player player;
    public FM_Monster monster;
    public FM_Grid grid;

    FM_GameObject[] gameObjects;
    //List<FM_GameObject> gameObjects;
    int startProjectileIndex = 2;

    //we must keep track of where the bullets and bombs are as these are not fully part of staterep
    //only their existence.

    public FM_Game(FM_Grid g, FM_GameObject[] gameObjs/*, AIState currentState*/)
    {
        grid = g;

        gameObjects = gameObjs;
        player = (FM_Player)gameObjects[0]; //player is first
        monster = (FM_Monster)gameObjects[1]; //monster second

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
                player.OnCollisionEnter(monster);
            }
            //bullets, bombs and walls after
            for (int j = startProjectileIndex; j < gameObjects.Length; j++)
            {
                if (gameObjects[j].IsAlive()) //if not alive don't bother
                {
                    //intersecting with player
                    if (gameObjects[j].IntersectsWith(player))
                    {
                        gameObjects[j].OnCollisionEnter(player);
                    }
                    //intersecting with monster (remember bombs can intersect both at the same time)
                    if (gameObjects[j].IntersectsWith(monster))
                    {
                        gameObjects[j].OnCollisionEnter(monster);
                    }
                }
            }
        }
    }

    //Update game based on the new state representation given by FM
    public void UpdateStateRep(float[] currentState)
    {
        stateRep = currentState;

        //monster
        monster.SetPosition(new Vector2(stateRep[3], stateRep[4]));
        float degrees = stateRep[5];
        monster.GetMovementComponent().SetDir((float)Math.Cos(degrees * Math.PI / 180), (float)Math.Sin(degrees * Math.PI / 180)); // degrees to vector
        monster.GetHealthComponent().SetHealth((int)stateRep[14]);

        //player
        player.SetPosition(new Vector2(stateRep[24], stateRep[25]));
        degrees = stateRep[26];
        player.GetMovementComponent().SetDir((float)Math.Cos(degrees * Math.PI / 180), (float)Math.Sin(degrees * Math.PI / 180));
        monster.GetHealthComponent().SetHealth((int)stateRep[27]);

        //bullets + bombs
        //TODO
    }

    //Gets the current state based on the state rep
    //Call this only after UpdateGame().
    //TODO: the rotation might be wrong because I am considering origin looking at right
    //the game itself might make it looking at left...
    //return the new state after updates
    public float[] GetNextState()
    {
        //game has been updated, update state representation



        Vector2 origin = new Vector2(0.0f, 0.0f);
        
        //monster
        stateRep[3] = monster.GetPosition().x;
        stateRep[4] = monster.GetPosition().y;
        stateRep[5] = Vector2.SignedAngle(origin, monster.GetMovementComponent().GetDir());
        stateRep[14] = monster.GetHealthComponent().GetHealth();

        //player
        stateRep[24] = player.GetPosition().x;
        stateRep[25] = player.GetPosition().y;
        stateRep[26] = Vector2.SignedAngle(origin, player.GetMovementComponent().GetDir());
        stateRep[27] = player.GetHealthComponent().GetHealth();

        return stateRep;
    }
}
