using UnityEngine;

public abstract class FM_GameObject
{
    //type and status
    protected FM_GameObjectType type;
    protected bool alive;

    //position and movement related
    protected Vector2 position, velocity;
    protected float speed;

    //collision related
    protected FM_Collider collider;

    protected FM_GameObject(Vector2 pos, Vector2 vel, float sp, FM_GameObjectType tp, bool a)
    {
        position = pos;
        velocity = vel;
        speed = sp;
        type = tp;
        alive = a;
    }

    //Getting and setting variables
    void SetType(FM_GameObjectType tp) { type = tp; }
    public FM_GameObjectType GetType() { return type; }
    public void SetPosition(Vector2 pos) { position.x = pos.x; position.y = pos.y; }
    public Vector2 GetPosition() { return position; }
    public void SetVelocity(Vector2 v) { velocity.x = v.x; velocity.y = v.y; }
	public Vector2 GetVelocity() { return velocity; }
    public float GetSpeed() { return speed; }
    public FM_Collider GetCollider() { return collider; }


    //Helping functions
    bool IsAlive() { return alive; }
    void DeleteGameObject() { alive = false; }

    public abstract void Update(FM_Game game, float elapsed = 1.0f);
}

public enum FM_GameObjectType
{
    Player,
    Monster,
    Bullet,
    Bomb
}