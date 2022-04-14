using UnityEngine;

public class FM_Collider
{
    //consider creating a super class for "Shape"?
    //A bit of a hack at the moment.
    private FM_Collider_Shape shape;
    private FM_Rectangle box;
    private Vector2 size;

    private FM_Circle circle;
    private float radius;


    public FM_Collider(FM_Rectangle b, Vector2 sz)
    {
        box = b;
        size = sz;
        shape = FM_Collider_Shape.Rectangle;
    }

    public FM_Collider(FM_Circle cir, float rad)
    {
        circle = cir;
        radius = rad;
        shape = FM_Collider_Shape.Circle;
    }

    public void Update(FM_GameObject gameObject)
    {
        if (shape == FM_Collider_Shape.Rectangle)
        {
            Vector2 bL = new Vector2(gameObject.GetPosition().x - size.x, gameObject.GetPosition().y - size.y);
            Vector2 tR = new Vector2(gameObject.GetPosition().x + size.x, gameObject.GetPosition().y + size.y);
            SetBox(bL, tR);
        }
        else
        {
            Vector2 c = new Vector2(gameObject.GetPosition().x, gameObject.GetPosition().y);
            SetCircle(c, radius);
        }
    }

    public Vector2 GetSize() { return size; }
    public FM_Collider_Shape GetShape() { return shape; }

    public FM_Rectangle GetBox() { return box; }
    public void SetBox(Vector2 bottomLeft, Vector2 topRight)
    {
        box.SetBottomLeft(bottomLeft);
        box.SetTopRight(topRight);
    }

    public FM_Circle GetCircle() { return circle; }
    public void SetCircle(Vector2 center, float radius)
    {
        circle.SetCenter(center);
        circle.SetRadius(radius);
    }

    //Can be optimised as well...
    public bool Intersects(FM_Collider other)
    {
        if (shape == FM_Collider_Shape.Rectangle && other.GetShape() == FM_Collider_Shape.Rectangle)
        {
            return other.GetBox().Intersects(GetBox());
        }
        else if (shape == FM_Collider_Shape.Rectangle && other.GetShape() == FM_Collider_Shape.Circle)
        {
            return other.GetCircle().Intersects(GetBox());
        }
        else if (shape == FM_Collider_Shape.Circle && other.GetShape() == FM_Collider_Shape.Rectangle)
        {
            return circle.Intersects(other.GetBox());
        }
        else if (shape == FM_Collider_Shape.Circle && other.GetShape() == FM_Collider_Shape.Circle)
        {
            return other.GetCircle().Intersects(GetCircle());
        }

        return false;
    }
}

public enum FM_Collider_Shape
{
    Rectangle,
    Circle
}
