//using System.Collections;
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

    List<FM_GameObject> gameObjects;

    //we must keep track of where the bullets and bombs are as these are not fully part of staterep
    //only their existence.

    public FM_Game(FM_Player p, FM_Monster m, FM_Grid g/*, AIState currentState*/)
    {
        grid = g;
        gameObjects = new List<FM_GameObject>();
        
        player = p;
        gameObjects.Add(player);
        monster = m;
        gameObjects.Add(monster);
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


    public void CreateGameObjectAt(Vector2 pos)
    {
        
    }

    public void AddGameObject(FM_GameObject obj)
    {
        gameObjects.Add(obj);
    }

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
            obj.Update(this, 1.0f);
        }

        //Checks on collisions (mostly if things are colliding with bullets and bombs to delete them
        //and add stuff to player

        //Checks if something is deleted and eliminates it


        //graphics update

    }

    //public FM_GameObject GetEntity to get an entity with a specific index.
}
