using UnityEngine;

/*Possible Improvement: FM_Wall is a game object as well,
 *just not a moving one...one can always have a super class
 *'FM_MovingGameObject' or something, which inherits from 
 *'FM_GameObject' but has all the velocity/direction code. 
 *Player, monster, bullets, bombs would inherit
 *from 'moving objects' and wall would simply inherit from
 *game object.*/
public class FM_Wall
{
    Vector2 position, size;
    FM_Collider collider;

    public FM_Wall(Vector2 pos, Vector2 sz)
    {
        position = pos;
        size = sz;

        //Make the rectangle box (collider)
        Vector2 bL = new Vector2(position.x - size.x, position.y - size.y);
        Vector2 tR = new Vector2(position.x + size.x, position.y + size.y);

        collider = new FM_Collider(new FM_Rectangle(bL, tR), size);        
    }

    public FM_Collider GetRecCollider() { return collider; }
    public Vector2 GetPosition() { return position; }
    public Vector2 GetSize() { return size; }

  
}
