using UnityEngine;

public class FM_Monster : FM_GameObject
{
    float size = 0.5f; //radius

    public FM_Monster(Vector2 _position, float _speed, float _rotationSpeed, FM_GameObjectType _type, bool _alive) :
        base(_position, _speed, _rotationSpeed, _type, _alive)
    {
        collider = new FM_Collider(new FM_Circle(position, size), size);
    }

    public override void Update(FM_Game game, float elapsed = 1.0f)
    {
        movement.Update(this, game);
        collider.Update(this);
    }
}
