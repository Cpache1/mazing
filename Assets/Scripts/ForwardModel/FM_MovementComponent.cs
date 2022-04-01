using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM_MovementComponent
{
    private Vector2 velocity, direction;
    private float speed, rotationSpeed;

    public FM_MovementComponent(float sp, float r_sp)
    {
        speed = sp;
        rotationSpeed = r_sp;
    }

    public Vector2 GetVel() { return velocity; }
    public void SetVel(float x, float y) { velocity.x = x; velocity.y = y; }
    public Vector2 GetDir() { return direction; }
    public void SetDir(float x, float y) { direction.x = x; direction.y = y; }
    public float GetSpeed() { return speed; }


    public void Update(FM_GameObject obj, FM_Game game)
    {
        //Updating position
        Vector2 oldPos = new Vector2(obj.GetPosition().x, obj.GetPosition().y);

        Vector3 newPos = Vector3.MoveTowards(new Vector3(obj.GetPosition().x, obj.GetPosition().y, 0.0f),
            new Vector3(obj.GetPosition().x + velocity.x, obj.GetPosition().y + velocity.y, 0.0f), speed * Time.deltaTime);
        obj.SetPosition(new Vector2(newPos.x, newPos.y));

        //This is a bit of a hack...
        //collisions with walls as it would be the only one that doesn't "go over"
        obj.GetCollider();
        if (!CanMakeMove(obj, game.grid))
        {
            obj.SetPosition(oldPos);
        }

        //Updating rotation
        UpdateRotation(obj);
    }

    private bool CanMakeMove(FM_GameObject obj, FM_Grid grid)
    {
        FM_Collider collider = obj.GetCollider();
        collider.Update(obj);

        for (int i = 0; i < grid.walls.Length; i++)
        {
            if (collider.Intersects(grid.walls[i].GetRecCollider()))
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateRotation(FM_GameObject obj)
    {
        //convertions being made to use Unity functions
        Vector3 tempVel = new Vector3(velocity.x, velocity.y, 0.0f);
        Vector3 tempCurrentDir = new Vector3(direction.x, direction.y, 0.0f);

        //The step size is the rotation speed times frame time.
        float singleStep = rotationSpeed * Time.deltaTime;

        //check if opposite directions - when it needs to turn exactly 180 degrees it breaks
        //gave a padding of 0.1
        if(Vector3.Dot(tempVel, tempCurrentDir) < -0.9)
        {
            Vector2 temp = RotateLeft(new Vector2(tempCurrentDir.x, tempCurrentDir.y));
            tempVel = new Vector3(temp.x, temp.y, 0.0f);
        }

        //rotate the vector towards the target direction which is velocity
        //(it turns slowly towards where it is going, which is provided by the velocity vector)
        Vector3 newDir = Vector3.RotateTowards(tempCurrentDir, tempVel, singleStep, 0.0f);
        SetDir(newDir.x, newDir.y);
    }

    //Rotates left or right by 90 degrees
    Vector2 RotateLeft(Vector2 v)
    {

        return new Vector2(v.y * -1.0f, v.x);

    }

    Vector2 RotateRight(Vector2 v)
    {

        return new Vector2(v.y, v.x * -1.0f);

    }
}
