using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM_MovementComponent
{
    private Vector2 velocity, direction;
    private float speed, rotationSpeed;
    private bool monsterCollided;

    public FM_MovementComponent(float sp, float r_sp)
    {
        speed = sp;
        rotationSpeed = r_sp;
        monsterCollided = false;
    }

    public Vector2 GetVel() { return velocity; }
    public void SetVel(float x, float y) { velocity.x = x; velocity.y = y; }
    public Vector2 GetDir() { return direction; }
    public void SetDir(float x, float y) { direction.x = x; direction.y = y; }
    public float GetSpeed() { return speed; }
    public void SetSpeed(float s) { speed = s; }
    public float GetRotationSpeed() { return rotationSpeed; }
    public void SetRotationSpeed(float rS) { rotationSpeed = rS; }
    public bool DidMonsterCollide() { return monsterCollided; }


    public void Update(FM_GameObject obj, FM_Game game)
    {
        monsterCollided = false;

        //Updating position
        Vector2 oldPos = new Vector2(obj.GetPosition().x, obj.GetPosition().y);

        Vector3 newPos = Vector3.MoveTowards(new Vector3(obj.GetPosition().x, obj.GetPosition().y, 0.0f),
            new Vector3(obj.GetPosition().x + velocity.x, obj.GetPosition().y + velocity.y, 0.0f), speed * Time.deltaTime);
        obj.SetPosition(new Vector2(newPos.x, newPos.y));

        //This is a bit of a hack...
        //collisions with walls... 
        obj.GetCollider();
        if (!CanMakeMove(obj, game.grid))
        {
            //... as a player or monster it goes "back" so they don't "go over"
            if (obj.GetType() == FM_GameObjectType.Player || obj.GetType() == FM_GameObjectType.Monster)
            {
                obj.SetPosition(oldPos);

                if (obj.GetType() == FM_GameObjectType.Monster)
                {
                    monsterCollided = true;
                }

            }
            else if (obj.GetType() == FM_GameObjectType.Bomb) //if it is a bomb just detonate it
            {
                FM_Bomb bomb = (FM_Bomb)obj;
                bomb.Detonate();
            }
            else //it's a bullet, destroy it
            {
                obj.DeleteGameObject();
            }
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

    //Rotates left by 90 degrees
    Vector2 RotateLeft(Vector2 v)
    {
        return new Vector2(v.y * -1.0f, v.x); //or left
    }
    //Rotates right by 90 degress 
    Vector2 RotateRight(Vector2 v)
    {
        return new Vector2(v.y, v.x * -1.0f);
    }
}
