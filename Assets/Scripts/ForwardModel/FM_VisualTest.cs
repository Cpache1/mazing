﻿/*ATTENTION: 
 * If the game isn't running (you're in the editor and haven't clicked the 'Play' button)
 * it will throw NullReferenceExceptions if you have the FM_VisualTest active and selected. 
 * This is because of how OnDrawGizmosSelected() works. FM_VisualTest gameObject should 
 * only be active as a visual aid if anything is changed in the forward model. This was
 * intended only as a way of testing that the FM scripts are outputting what is needed. 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM_VisualTest : MonoBehaviour
{
    public bool moveLeft = false;
    public bool moveRight = false;
    public bool moveUp = false;
    public bool moveDown = false;

    public bool shoot = false;
    public bool bomb = false;
    public bool dash = false;

    FM_Game game;

    public float padding = 50.0f;

    // Start is called before the first frame update
    void Start()
    {
        FM_Grid grid = new FM_Grid(GameObject.FindGameObjectsWithTag("wall"));

        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        float speed = GameObject.Find("PlayerController").GetComponent<PlayerController>().movementSpeed;
        float rotationSpeed = GameObject.Find("PlayerController").GetComponent<PlayerController>().rotationSpeed;
        FM_Player player = new FM_Player(playerPos, speed, rotationSpeed, FM_GameObjectType.Player, true);
        player.GetMovementComponent().SetVel(0, 0);
        player.GetMovementComponent().SetDir(-1.0f, 0.0f);

        Vector3 monsterPos = GameObject.FindGameObjectWithTag("AI").transform.position;
        float mSpeed = GameObject.Find("Monster").GetComponent<Movement>().speed;
        float mRotationSpeed = GameObject.Find("Monster").GetComponent<Movement>().rotationSpeed;
        FM_Monster monster = new FM_Monster(monsterPos, mSpeed, mRotationSpeed, FM_GameObjectType.Monster, true);
        monster.GetMovementComponent().SetVel(0, 0);
        monster.GetMovementComponent().SetDir(0, 1);

        
        FM_GameObject[] gameObjs = CreateGameObjs(player, monster);
        game = new FM_Game(grid, gameObjs);
    }

    // Update is called once per frame
    void Update()
    {
        float x = 0.0f;
        float y = 0.0f;

        if (moveLeft)
        {
            x = -1.0f;
        }
        if (moveRight)
        {
            x = 1.0f;
        }
        if (moveDown)
        {
            y = -1.0f;
        }
        if (moveUp)
        {
            y = 1.0f;
        }
        
        if(shoot)
        {
            shoot = false;
            game.GiveGunInputs(true, false);
        }
        if(bomb)
        {
            bomb = false;
            game.GiveGunInputs(false, true);
        }
        if(dash)
        {
            dash = false;
            game.GiveMoveInputs(true);
        }

        if (game != null)
        {
            float[] action = { x, y };
            game.Apply(action, game.player);


            game.UpdateGame();
        }
    }

    private void OnDrawGizmosSelected()
    {
        DrawWalls();
        DrawGameObjects();
    }

    private void DrawWalls()
    {
        for (int i = 0; i < game.grid.walls.Length; i++)
        {
            FM_Collider wallCollider = game.grid.walls[i].GetRecCollider();
            FM_Rectangle rec = wallCollider.GetBox();
            //it converts to Vector3 and keeps z = 0
            Vector3 bottomLeft = rec.GetBottomLeft();
            Vector3 topRight = rec.GetTopRight();
            Vector3 sz = wallCollider.GetSize();

            // Draws a green line from a point to another
            // This can be done as boxes directly, but this 
            // way tests that topLeft and bottomRight points 
            // are correct and properly translated.
            Gizmos.color = Color.green;
            Gizmos.DrawLine(bottomLeft + new Vector3(padding, 0, 0), bottomLeft + new Vector3(sz.x * 2 + padding, 0, 0)); //bL to bR
            Gizmos.DrawLine(bottomLeft + new Vector3(padding, 0, 0), bottomLeft + new Vector3(padding, sz.y * 2, 0));// bL to tL
            Gizmos.DrawLine(bottomLeft + new Vector3(padding, sz.y * 2, 0), topRight + new Vector3(padding, 0, 0)); // tL to tR
            Gizmos.DrawLine(topRight + new Vector3(padding, 0, 0), bottomLeft + new Vector3(sz.x * 2 + padding, 0, 0)); //tR to bR
        }
    }

    private void DrawGameObjects()
    {
        for (int i = 0; i < game.GetGameObjects().Length; i++)
        {
            FM_GameObject obj = game.GetGameObjects()[i];

            if (obj.IsAlive())
            {
                if (obj.GetType() == FM_GameObjectType.Player || obj.GetType() == FM_GameObjectType.Monster)
                {
                    Vector3 c = obj.GetCollider().GetCircle().GetCenter();
                    Gizmos.color = obj.GetType() == FM_GameObjectType.Player ? Color.blue : Color.red;
                    Gizmos.DrawSphere(c + new Vector3(padding, 0, 0),
                        obj.GetCollider().GetCircle().GetRadius());

                    Vector3 d = obj.GetMovementComponent().GetDir();
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(c + new Vector3(padding, 0, 0),
                        c + d + new Vector3(padding, 0, 0));
                }
                else
                {
                    FM_Collider collider = obj.GetCollider();
                    FM_Rectangle rec = collider.GetBox();
                    //it converts to Vector3 and keeps z = 0
                    Vector3 bottomLeft = rec.GetBottomLeft();
                    Vector3 topRight = rec.GetTopRight();
                    Vector3 sz = collider.GetSize();

                    // Draws a green line from a point to another
                    // This can be done as boxes directly, but this 
                    // way tests that topLeft and bottomRight points 
                    // are correct and properly translated.
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(bottomLeft + new Vector3(padding, 0, 0), bottomLeft + new Vector3(sz.x * 2 + padding, 0, 0)); //bL to bR
                    Gizmos.DrawLine(bottomLeft + new Vector3(padding, 0, 0), bottomLeft + new Vector3(padding, sz.y * 2, 0));// bL to tL
                    Gizmos.DrawLine(bottomLeft + new Vector3(padding, sz.y * 2, 0), topRight + new Vector3(padding, 0, 0)); // tL to tR
                    Gizmos.DrawLine(topRight + new Vector3(padding, 0, 0), bottomLeft + new Vector3(sz.x * 2 + padding, 0, 0)); //tR to bR

                    if(obj.GetType() == FM_GameObjectType.Bomb)
                    {
                        DrawTargets((FM_Bomb)obj);
                    }
                }
            }
        }
    }

    private void DrawTargets(FM_Bomb bomb)
    {
        if (bomb.hasTarget)
        {
            FM_Rectangle rec = bomb.GetTarget().GetBox();
            //it converts to Vector3 and keeps z = 0
            Vector3 bottomLeft = rec.GetBottomLeft();
            Vector3 topRight = rec.GetTopRight();
            Vector3 sz = bomb.GetTarget().GetSize();

            // Draws a green line from a point to another
            // This can be done as boxes directly, but this 
            // way tests that topLeft and bottomRight points 
            // are correct and properly translated.
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(bottomLeft + new Vector3(padding, 0, 0), bottomLeft + new Vector3(sz.x * 2 + padding, 0, 0)); //bL to bR
            Gizmos.DrawLine(bottomLeft + new Vector3(padding, 0, 0), bottomLeft + new Vector3(padding, sz.y * 2, 0));// bL to tL
            Gizmos.DrawLine(bottomLeft + new Vector3(padding, sz.y * 2, 0), topRight + new Vector3(padding, 0, 0)); // tL to tR
            Gizmos.DrawLine(topRight + new Vector3(padding, 0, 0), bottomLeft + new Vector3(sz.x * 2 + padding, 0, 0)); //tR to bR
        }
    }

    private FM_GameObject[] CreateGameObjs(FM_Player p, FM_Monster m)
    {
        Vector2 vec = new Vector2(0.0f, 0.0f);

        int startingBullets = 5;
        float bulletSpeed = 1000.0f;

        int startingBombs = 3;
        float bombSpeed = 500.0f;

        float rotationSpeed = 0.0f;

        FM_GameObject[] objs = new FM_GameObject[2 + startingBullets + startingBombs];
        objs[0] = p;
        objs[1] = m;

        for (int i = 2; i < objs.Length; i++)
        {
            if (i < 2 + startingBullets)
            {
                objs[i] = new FM_Bullet(vec, bulletSpeed, rotationSpeed, FM_GameObjectType.Bullet, false);
            }
            else
            {
                objs[i] = new FM_Bomb(vec, bombSpeed, rotationSpeed, FM_GameObjectType.Bomb, false);
            }
        }

        return objs;

    }

}
