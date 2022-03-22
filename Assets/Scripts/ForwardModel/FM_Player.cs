using UnityEngine;

public class FM_Player
{
    Vector2 position;
    float size;
    FM_Collider circle;

    public FM_Player(Vector2 pos, float radius)
    {
        position = pos;
        size = radius;

        //Make the circle (collider)
        circle = new FM_Collider(new FM_Circle(position, size), size);
    }

    public Vector2 GetPosition() { return position; }
    public FM_Collider GetCircleCollider() { return circle; }
}
