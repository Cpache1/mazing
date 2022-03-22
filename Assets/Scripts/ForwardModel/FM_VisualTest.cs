/*ATTENTION: 
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
    FM_Grid grid;
    FM_Player player;

    public float padding = 50.0f;

    // Start is called before the first frame update
    void Start()
    {
        grid = new FM_Grid(GameObject.FindGameObjectsWithTag("wall"));
        
        Vector3 playerPos = GameObject.Find("PlayerController").transform.position;
        player = new FM_Player(playerPos, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        DrawWalls();
        DrawPlayer();
    }

    private void DrawWalls()
    {
        for (int i = 0; i < grid.walls.Length; i++)
        {
            FM_Collider box = grid.walls[i].GetBox();
            FM_Rectangle rec = box.GetBox();
            //it converts to Vector3 and keeps z = 0
            Vector3 topLeft = rec.GetTopLeft();
            Vector3 botRight = rec.GetBottomRight();
            Vector3 sz = box.GetSize();

            // Draws a green line from a point to another
            // This can be done as boxes directly, but this 
            // way tests that topLeft and bottomRight points 
            // are correct and properly translated.
            Gizmos.color = Color.green;
            Gizmos.DrawLine(topLeft + new Vector3(padding, 0, 0), topLeft + new Vector3(sz.x * 2 + padding, 0, 0)); //tL to tR
            Gizmos.DrawLine(topLeft + new Vector3(padding, 0, 0), topLeft + new Vector3(padding, -sz.y * 2, 0)); // tL to bL
            Gizmos.DrawLine(botRight + new Vector3(-sz.x * 2 + padding, 0, 0), botRight + new Vector3(padding, 0, 0)); //bL to bR
            Gizmos.DrawLine(botRight + new Vector3(padding, 0, 0), botRight + new Vector3(padding, sz.y * 2, 0)); //bR to tR
        }
    }

    private void DrawPlayer()
    {
        Vector3 c = player.GetCircleCollider().GetCircle().GetCenter();
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere( c + new Vector3(padding, 0, 0), 
            player.GetCircleCollider().GetCircle().GetRadius());
    }

    private void DrawMonster()
    {

    }

    private void DrawBullets()
    {

    }

    private void DrawBombs()
    {

    }
}
