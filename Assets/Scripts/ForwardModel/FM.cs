using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM : MonoBehaviour
{
    //Forward Model
    FM_Game game; 
    FM_GameObject[] gameObjs;
    LevelManager levelManager; //to retrieve live game info

    // Start is called before the first frame update
    void Start()
    {
        levelManager = transform.GetComponent<LevelManager>();

        FM_Grid grid = new FM_Grid(GameObject.FindGameObjectsWithTag("wall"));

        Vector3 playerPos = levelManager.player.transform.position;
        float speed = levelManager.player.GetComponent<PlayerController>().movementSpeed;
        float rotationSpeed = levelManager.player.GetComponent<PlayerController>().rotationSpeed;
        FM_Player player = new FM_Player(playerPos, speed, rotationSpeed, FM_GameObjectType.Player, true);
        player.GetMovementComponent().SetVel(0, 0);
        player.GetMovementComponent().SetDir(-1, 0);
        //gameObjs.Add(player);

        Vector3 monsterPos = levelManager.agent.transform.position;
        float mSpeed = levelManager.agent.GetComponent<Movement>().speed;
        float mRotationSpeed = levelManager.agent.GetComponent<Movement>().rotationSpeed;
        FM_Monster monster = new FM_Monster(monsterPos, mSpeed, mRotationSpeed, FM_GameObjectType.Monster, true);
        monster.GetMovementComponent().SetVel(0, 0);
        monster.GetMovementComponent().SetDir(0, 1);
        

        gameObjs = CreateGameObjs(player, monster);
        game = new FM_Game(grid, gameObjs);
    }

    private FM_GameObject[] CreateGameObjs(FM_Player p, FM_Monster m)
    {
        Vector2 vec = new Vector2(0.0f, 0.0f);

        int startingBullets = 5;
        float bulletSpeed = 1000.0f;

        int startingBombs = 3;
        float bombSpeed = 500.0f;

        float rotationSpeed = 0.0f;

        gameObjs = new FM_GameObject[2 + startingBullets + startingBombs];
        gameObjs[0] = p;
        gameObjs[1] = m;

        for (int i = 2; i < gameObjs.Length; i++)
        {
            if (i < 2 + startingBullets)
            {
                gameObjs[i] = new FM_Bullet(vec, bulletSpeed, rotationSpeed, FM_GameObjectType.Bullet, false);
            }
            else
            {
                gameObjs[i] = new FM_Bomb(vec, bombSpeed, rotationSpeed, FM_GameObjectType.Bomb, false);
            }
        }

        return gameObjs;

    }

    public float[] UpdateGameState(float[] action, float[] currentState, List<ProjectileStruct> projectiles, int idx)
    {
        //change FM_Game with new state representation
        game.UpdateStateRep(currentState, projectiles);

        //apply an action to it
        if (idx == 0)
        {
            game.Apply(action, game.player);
        }
        else if (idx == 1)
        {
            game.Apply(action, game.monster);
        }

        //tick once 
        game.UpdateGame();

        //get the resulting state and return it
        return game.GetNextState();
    }

    public List<ProjectileStruct> GetProjectileStructs()
    {
        List<ProjectileStruct> copySt = new List<ProjectileStruct>();
        foreach (ProjectileStruct ps in game.GetProjectileStructs())
        {
            ProjectileStruct psCopy = ps.GetCopy(); 
            copySt.Add(psCopy);
        }
        return copySt;
    }

    public FM_Game GetGame() { return game; }
}
