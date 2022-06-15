using UnityEngine;

public class FM_Player : FM_GameObject
{
    public bool burning = false;
    int startingHealth = 100;
    int maxHealth = 100;
    FM_HealthComponent health;

    float size = 0.5f; //radius
    FM_GunControlComponent gunControl;

    public FM_Player(Vector2 _position, float _speed, float _rotationSpeed, FM_GameObjectType _type, bool _alive) :
        base(_position, _speed, _rotationSpeed, _type, _alive) 
    {
        collider = new FM_Collider(new FM_Circle(position, size), size);
        health = new FM_HealthComponent(startingHealth, maxHealth);
        gunControl = new FM_GunControlComponent();
    }
    
    //public int GetHealth() { return health; }

    public override void OnCollisionEnter(FM_GameObject other, FM_Game game)
    {
        if(other.GetType()==FM_GameObjectType.Monster)
        {
            GetHealthComponent().AddHealth(-500);
        }

        //check its state
        CheckState();
    }

    public override void Update(FM_Game game, float elapsed = 1.0f)
    {
        gunControl.Update(this, game);
        movement.Update(this, game);
        collider.Update(this);
        health.Update();
        CheckState();
    }

    public FM_GunControlComponent GetGunControl() { return gunControl; }
    public FM_HealthComponent GetHealthComponent() { return health; }

    public void CheckState()
    {
        if (GetHealthComponent().GetHealth() == 0)
        {
            alive = false;
        }
    }

    public void Dash() { movement.playerIsDashing = true; }
}
