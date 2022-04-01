﻿using UnityEngine;

public class FM_Player : FM_GameObject
{
    int health;
    int maxHealth;
    float size = 0.5f; //radius

    public FM_Player(Vector2 _position, float _speed, float _rotationSpeed, FM_GameObjectType _type, bool _alive) :
        base(_position, _speed, _rotationSpeed, _type, _alive) 
    {
        collider = new FM_Collider(new FM_Circle(position, size), size);
    }
    
    //public int GetHealth() { return health; }

    public override void Update(FM_Game game, float elapsed = 1.0f)
    {
        movement.Update(this, game);
        collider.Update(this);
    }

    /*private void UpdateMovement(FM_Game game, float elapsed = 1.0f)
    {
        Vector2 oldPos = new Vector2(GetPosition().x, GetPosition().y);

        Vector3 newPos = Vector3.MoveTowards(new Vector3(GetPosition().x, GetPosition().y, 0.0f),
            new Vector3(GetPosition().x + GetVelocity().x, GetPosition().y + GetVelocity().y, 0.0f), speed * Time.deltaTime);
        this.SetPosition(new Vector2(newPos.x, newPos.y));

        //This is a bit of a hack...
        //collisions with walls as it would be the only one that doesn't "go over"
        if (!CanMakeMove(game.grid))
        {
            SetPosition(oldPos);
        }
    }

    private bool CanMakeMove(FM_Grid grid)
    {
        collider.Update(this);

        for (int i = 0; i < grid.walls.Length; i++)
        {
            if(collider.Intersects(grid.walls[i].GetRecCollider()))
            {
                return false;
            }
        }
        return true;
    }*/

}
