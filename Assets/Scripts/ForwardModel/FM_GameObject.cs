using UnityEngine;

public class FM_GameObject
{
    FM_GameObjectType type;
    Vector2 position;
    Vector2 velocity;

    FM_Collider collider;


    public Vector2 GetPosition() { return position; }
}

public enum FM_GameObjectType
{
    Player,
    Monster,
    Bullet,
    Bomb
}