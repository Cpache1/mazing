using UnityEngine;

public class FM_Bullet : FM_GameObject
{
    Vector2 size;

    int bulletDamage = 5;

    public FM_Bullet(Vector2 _position, float _speed, float _rotationSpeed, FM_GameObjectType _type, bool _alive, Vector2 _size) :
        base(_position, _speed, _rotationSpeed, _type, _alive)
    {
        size = _size;
        Vector2 bL = new Vector2(position.x - size.x, position.y - size.y);
        Vector2 tR = new Vector2(position.x + size.x, position.y + size.y);

        collider = new FM_Collider(new FM_Rectangle(bL, tR), size);
    }

    public override void OnCollisionEnter(FM_GameObject other, FM_Game game)
    {
        if (other.GetType() == FM_GameObjectType.Monster)
        {
            DeleteGameObject();
            FM_Monster monster = (FM_Monster)other;
            monster.GetHealthComponent().AddHealth(-bulletDamage);
            game.AddScore(bulletDamage);
        }
    }

    public override void Update(FM_Game game, float elapsed = 1.0f)
    {
        if (alive)
        {
            movement.Update(this, game);
            collider.Update(this);
        }
    }
}
