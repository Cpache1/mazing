using UnityEngine;

public class FM_Monster : FM_GameObject
{
    int startingHealth = 100;
    int maxHealth = 100;
    FM_HealthComponent health;

    float size = 0.5f; //radius

    public FM_Monster(Vector2 _position, float _speed, float _rotationSpeed, FM_GameObjectType _type, bool _alive) :
        base(_position, _speed, _rotationSpeed, _type, _alive)
    {
        collider = new FM_Collider(new FM_Circle(position, size), size);
        health = new FM_HealthComponent(startingHealth, maxHealth);
    }

    public override void OnCollisionEnter(FM_GameObject other, FM_Game game)
    {
        /*if (other.GetType() == FM_GameObjectType.Player)
        {
            DeleteGameObject();
        }*/
    }

    public override void Update(FM_Game game, float elapsed = 1.0f)
    {
        movement.Update(this, game);
        collider.Update(this);
        health.Update();
        CheckState();
    }

    public FM_HealthComponent GetHealthComponent() { return health; }

    void CheckState()
    {
        if (GetHealthComponent().GetHealth() == 0)
        {
            alive = false;
        }
    }
}
