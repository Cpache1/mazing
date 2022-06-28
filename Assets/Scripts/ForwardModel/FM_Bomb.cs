using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM_Bomb : FM_GameObject
{
    Vector2 size = new Vector2(0.1f, 0.1f);

    FM_Collider target;
    public bool hasTarget = false;
    bool detonated = false;
    public int ttl = 400;
    int fireDamage = 1;

    public FM_Bomb(Vector2 _position, float _speed, float _rotationSpeed, FM_GameObjectType _type, bool _alive) :
        base(_position, _speed, _rotationSpeed, _type, _alive)
    {
        Vector2 bL = new Vector2(position.x - size.x, position.y - size.y);
        Vector2 tR = new Vector2(position.x + size.x, position.y + size.y);

        collider = new FM_Collider(new FM_Rectangle(bL, tR), size);
    }

    public void SetTarget(Vector2 t) 
    {
        Vector2 targetSize = new Vector2(1, 1);

        Vector2 bL = new Vector2(t.x - targetSize.x, t.y - targetSize.y);
        Vector2 tR = new Vector2(t.x + targetSize.x, t.y + targetSize.y);

        target = new FM_Collider(new FM_Rectangle(bL, tR), targetSize);
        hasTarget = true;
    }
    public FM_Collider GetTarget() { return target; }

    public bool hasDetonated() { return detonated; }

    public void Detonate()
    {
        hasTarget = false;
        detonated = true;
        ttl = 400;

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
                monster.burning = true;
                monster.botTakingRiskyPath = true;
                monster.GetHealthComponent().AddHealth(-fireDamage);
                game.AddScore(fireDamage);
            }

            if (other.GetType() == FM_GameObjectType.Player)
            {
                FM_Player player = (FM_Player)other;
                player.burning = true;
                player.GetHealthComponent().AddHealth(-fireDamage);
            }
        }
    }

    private void CheckItHitTarget()
    {
        //does collider intersect with target and is !detonated?
        if (GetCollider().Intersects(target) && hasTarget)
        {
            Detonate();
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
            CheckItHitTarget();
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
        hasTarget = false;
        DeleteGameObject();
        detonated = false;
        ttl = 400;

        size = new Vector2(0.1f, 0.1f);
        Vector2 bL = new Vector2(position.x - size.x, position.y - size.y);
        Vector2 tR = new Vector2(position.x + size.x, position.y + size.y);

        collider = new FM_Collider(new FM_Rectangle(bL, tR), size);
    }
}
