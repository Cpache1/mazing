using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM_Bomb : FM_GameObject
{
    Vector2 size = new Vector2(0.1f, 0.1f);
    bool detonated = false;
    public int ttl = 10;

    int fireDamage = 2;

    public FM_Bomb(Vector2 _position, float _speed, float _rotationSpeed, FM_GameObjectType _type, bool _alive) :
        base(_position, _speed, _rotationSpeed, _type, _alive)
    {
        Vector2 bL = new Vector2(position.x - size.x, position.y - size.y);
        Vector2 tR = new Vector2(position.x + size.x, position.y + size.y);

        collider = new FM_Collider(new FM_Rectangle(bL, tR), size);
    }

    public bool hasDetonated() { return detonated; }

    public void Detonate()
    {
        detonated = true;
        ttl = 10;

        size = new Vector2(1, 1);
        Vector2 bL = new Vector2(position.x - size.x, position.y - size.y);
        Vector2 tR = new Vector2(position.x + size.x, position.y + size.y);

        collider = new FM_Collider(new FM_Rectangle(bL, tR), size);
    }

    public override void OnCollisionEnter(FM_GameObject other, FM_Game game)
    {
        if (!detonated)
        {
            if (other.GetType() == FM_GameObjectType.Monster)
            {
                Detonate();
            }
        }
        else //it's a fire now
        {
            if (other.GetType() == FM_GameObjectType.Monster)
            {
                FM_Monster monster = (FM_Monster)other;
                monster.GetHealthComponent().AddHealth(-fireDamage);
                game.AddScore(fireDamage);
            }

            if (other.GetType() == FM_GameObjectType.Player)
            {
                FM_Player player = (FM_Player)other;
                player.GetHealthComponent().AddHealth(-fireDamage);
            }
        }
    }

    public override void Update(FM_Game game, float elapsed = 1.0f)
    {
        if (alive)
        {
            if (!detonated)
            {
                movement.Update(this, game);
            }
            else
            {
                UpdateState();
            }
            collider.Update(this);
        }
    }


    private void UpdateState()
    {
        ttl--;
        if (ttl <= 0)
        {
            ResetBomb();            
        }
    }

    public void ResetBomb()
    {
        DeleteGameObject();
        detonated = false;
        ttl = 10;

        size = new Vector2(0.1f, 0.1f);
        Vector2 bL = new Vector2(position.x - size.x, position.y - size.y);
        Vector2 tR = new Vector2(position.x + size.x, position.y + size.y);

        collider = new FM_Collider(new FM_Rectangle(bL, tR), size);
    }
}
