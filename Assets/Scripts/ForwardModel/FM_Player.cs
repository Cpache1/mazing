using UnityEngine;

public class FM_Player : FM_GameObject
{
    int health;
    int maxHealth;
    float size = 0.5f; //radius
    FM_GunControlComponent gunControl;

    public FM_Player(Vector2 _position, float _speed, float _rotationSpeed, FM_GameObjectType _type, bool _alive) :
        base(_position, _speed, _rotationSpeed, _type, _alive) 
    {
        collider = new FM_Collider(new FM_Circle(position, size), size);
        gunControl = new FM_GunControlComponent();
    }
    
    //public int GetHealth() { return health; }

    public override void OnCollisionEnter(FM_GameObject other)
    {
        if(other.GetType()==FM_GameObjectType.Monster)
        {

        }
    }

    public override void Update(FM_Game game, float elapsed = 1.0f)
    {
        gunControl.Update(this, game);
        movement.Update(this, game);
        collider.Update(this);
    }

    public FM_GunControlComponent GetGunControl() { return gunControl; }
}
