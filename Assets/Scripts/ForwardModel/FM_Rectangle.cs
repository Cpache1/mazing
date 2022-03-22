using UnityEngine;

public class FM_Rectangle
{
    private Vector2 topLeft;
    private Vector2 bottomRight;

    public FM_Rectangle()
    {
        topLeft = new Vector2(0.0f, 0.0f);
        bottomRight = new Vector2(0.0f, 0.0f);
    }

    public FM_Rectangle(Vector2 tL, Vector2 bR)
    {
        topLeft = tL;
        bottomRight = bR;
    }

    public void SetTopLeft(Vector2 tL) { topLeft = tL; }
    public Vector2 GetTopLeft() { return topLeft; }
    public void SetBottomRight(Vector2 bR) { bottomRight = bR; }
    public Vector2 GetBottomRight() { return bottomRight; }

    bool IsInside(double x, double y)
    {
        return (x >= topLeft.x && x <= bottomRight.x && y >= topLeft.y && y <= bottomRight.y); ;
    }

    bool IsInside(Vector2 point) { return IsInside(point.x, point.y); }

    public bool Intersects(FM_Rectangle rectangle)
    {
        if (IsInside(rectangle.topLeft.x, rectangle.topLeft.y)) return true;
        if (IsInside(rectangle.topLeft.x, rectangle.bottomRight.y)) return true;
        if (IsInside(rectangle.bottomRight.x, rectangle.topLeft.y)) return true;
        if (IsInside(rectangle.bottomRight.x, rectangle.bottomRight.y)) return true;

        return false;
    }
}
