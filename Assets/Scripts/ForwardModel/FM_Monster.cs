using UnityEngine;

public class FM_Monster : FM_GameObject
{
    public bool burning = false;
    public bool botTakingRiskyPath = false;

    int startingHealth = 100;
    int maxHealth = 100;

    public bool botSearching = false;
    public bool botSeeingPlayer = false;
    public int botSearchTurns = 0;
    public bool botChasingPlayer = false;
    public bool botLostPlayer = false;
    public bool botSpottedPlayer = false;

    FM_HealthComponent health;
    FM_FrustrationComponent frustration;

    float size = 0.48f; //radius

    public FM_Monster(Vector2 _position, float _speed, float _rotationSpeed, FM_GameObjectType _type, bool _alive) :
        base(_position, _speed, _rotationSpeed, _type, _alive)
    {
        collider = new FM_Collider(new FM_Circle(position, size), size);
        health = new FM_HealthComponent(startingHealth, maxHealth);
        frustration = new FM_FrustrationComponent();
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
        health.Update(false);
        frustration.Update();
        CheckState(game);
    }

    public FM_HealthComponent GetHealthComponent() { return health; }
    public FM_FrustrationComponent GetFrustrationComponent() { return frustration; }


    private void CheckState(FM_Game game)
    {
        if (GetHealthComponent().GetHealth() == 0)
        {
            alive = false;
        }

        float distance = (game.player.GetPosition() - GetPosition()).magnitude;
        if (botSearching)
        {
            if (distance < 10) //hearing radius
            {
                botSearching = false;
                botChasingPlayer = true;
                botSeeingPlayer = true;
                botLostPlayer = false;
                botSpottedPlayer = true;
            }
        }
        else
        {
            if (distance > 10)
            {
                botSearching = true;
                botChasingPlayer = false;
                botSeeingPlayer = false;
                botSearchTurns++;
                botLostPlayer = true;
                botSpottedPlayer = false;
            }
        }        
    }
}
