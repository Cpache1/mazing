using UnityEngine;

public abstract class FM_GameObject
{
    //type and status
    protected FM_GameObjectType type;
    protected bool alive;

    //position and movement related
    protected Vector2 position;
    protected FM_MovementComponent movement;
    protected float speed, rotationSpeed;

    //collision related
    protected FM_Collider collider;
    
    protected FM_GameObject(Vector2 pos, float sp, float r_sp, FM_GameObjectType tp, bool a)
    {
        position = pos;
        speed = sp;
        rotationSpeed = r_sp;
        type = tp;
        alive = a;
        movement = new FM_MovementComponent(speed, rotationSpeed);
    }

    //Getting and setting variables
    void SetType(FM_GameObjectType tp) { type = tp; }
    public FM_GameObjectType GetType() { return type; }
    public void SetPosition(Vector2 pos) { position.x = pos.x; position.y = pos.y; }
    public Vector2 GetPosition() { return position; }
    public FM_MovementComponent GetMovementComponent() { return movement; }
    public FM_Collider GetCollider() { return collider; }


    //Helping functions
    public bool IsAlive() { return alive; }
    public void DeleteGameObject() { alive = false; }
    public void Revive() { alive = true; }

    public abstract void Update(FM_Game game, float elapsed = 1.0f); //TODO, remove elapsed?
    public bool IntersectsWith(FM_GameObject other) { return GetCollider().Intersects(other.GetCollider()); }
    public abstract void OnCollisionEnter(FM_GameObject other);
}

public enum FM_GameObjectType
{
    Player,
    Monster,
    Bullet,
    Bomb
}