using UnityEngine;

public class FM_Monster : FM_GameObject
{
    float size = 0.5f; //radius

    public FM_Monster(Vector2 _position, Vector2 _velocity, float _speed, FM_GameObjectType _type, bool _alive) :
        base(_position, _velocity, _speed, _type, _alive)
    {
        collider = new FM_Collider(new FM_Circle(position, size), size);
    }


    public override void Update(FM_Game game, float elapsed = 1)
    {
        throw new System.NotImplementedException();
    }

}
