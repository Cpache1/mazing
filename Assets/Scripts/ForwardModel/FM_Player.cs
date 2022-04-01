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

    public override void Update(FM_Game game, float elapsed = 1.0f)
    {
        movement.Update(this, game);
        collider.Update(this);
    }

    public void Shoot(FM_Game game)
    {
        Vector2 pos = GetPosition() + GetMovementComponent().GetDir();
        FM_Bullet bullet = new FM_Bullet(pos, 1000.0f, 0.0f, FM_GameObjectType.Bullet, true, new Vector2(0.275f, 0.1f));
        //bullet.GetMovementComponent().SetDir(GetMovementComponent().GetDir());
        bullet.GetMovementComponent().SetVel(GetMovementComponent().GetDir().x, GetMovementComponent().GetDir().y);
        game.AddGameObject(bullet);
    }

}
