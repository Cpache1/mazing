using UnityEngine;

public class FM_Rectangle
{
    private Vector2 bottomLeft;
    private Vector2 topRight;

    public FM_Rectangle()
    {
        bottomLeft = new Vector2(0.0f, 0.0f);
        topRight = new Vector2(0.0f, 0.0f);
    }

    public FM_Rectangle(Vector2 bL, Vector2 tR)
    {
        bottomLeft = bL;
        topRight = tR;
    }

    public void SetTopRight(Vector2 tR) { topRight = tR; }
    public Vector2 GetTopRight() { return topRight; }
    public void SetBottomLeft(Vector2 bL) { bottomLeft = bL; }
    public Vector2 GetBottomLeft() { return bottomLeft; }

    bool IsInside(double x, double y)
    {
        return (x >= bottomLeft.x && x <= topRight.x && y >= bottomLeft.y && y <= topRight.y);
    }

    //bool IsInside(Vector2 point) { return IsInside(point.x, point.y); } //TODO: Delete?

    public bool Intersects(FM_Rectangle rectangle)
    {
        if (IsInside(rectangle.topRight.x, rectangle.topRight.y)) return true;
        if (IsInside(rectangle.topRight.x, rectangle.bottomLeft.y)) return true;
        if (IsInside(rectangle.bottomLeft.x, rectangle.topRight.y)) return true;
        if (IsInside(rectangle.bottomLeft.x, rectangle.bottomLeft.y)) return true;

        return false;
    }
}
