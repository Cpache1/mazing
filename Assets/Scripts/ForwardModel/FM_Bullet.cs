using UnityEngine;

public class FM_Bullet : FM_GameObject
{
    Vector2 size;

    public FM_Bullet(Vector2 _position, float _speed, float _rotationSpeed, FM_GameObjectType _type, bool _alive, Vector2 _size) :
        base(_position, _speed, _rotationSpeed, _type, _alive)
    {
        size = _size;
        Vector2 bL = new Vector2(position.x - size.x, position.y - size.y);
        Vector2 tR = new Vector2(position.x + size.x, position.y + size.y);

        collider = new FM_Collider(new FM_Rectangle(bL, tR), size);
    }

    //public int GetHealth() { return health; }

    public override void Update(FM_Game game, float elapsed = 1.0f)
    {
        movement.Update(this, game);
        collider.Update(this);
    }
}
