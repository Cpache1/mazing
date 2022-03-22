using UnityEngine;

public class FM_Wall
{
    Vector2 position, size;
    FM_Collider box;

    public FM_Wall(Vector2 pos, Vector2 sz)
    {
        position = pos;
        size = sz;

        //Make the rectangle box (collider)
        Vector2 tL = new Vector2(position.x - size.x, position.y + size.y);
        Vector2 bR = new Vector2(position.x + size.x, position.y - size.y);

        box = new FM_Collider(new FM_Rectangle(tL, bR), size);        
    }

    public FM_Collider GetBox() { return box; }
    public Vector2 GetPosition() { return position; }
    public Vector2 GetSize() { return size; }

  
}
