﻿using System;
using UnityEngine;

public class FM_Circle
{
    private Vector2 center;
    private float radius;

    public FM_Circle()
    {
        center = new Vector2(0.0f, 0.0f);
        radius = 0.5f; //radius for monster and player
    }

    public FM_Circle(Vector2 c, float r)
    {
        center = c;
        radius = r;
    }

    public void SetCenter(Vector2 c) { center = c; }
    public Vector2 GetCenter() { return center; }
    public void SetRadius(float r) { radius = r; }
    public float GetRadius() { return radius; }

    bool IsInside(double x, double y)
    {
        return (x - center.x) * (x - center.x) +
            (y - center.y) * (y - center.y) <= radius * radius;
    }

    bool IsInside(Vector2 point) { return IsInside(point.x, point.y); }

    public bool Intersects(FM_Circle circle)
    {
        double x = circle.center.x;
        double y = circle.center.y;
        float rad = circle.radius;

        double distance = Math.Sqrt(((x - center.x) * (x - center.x) +
            (y - center.y) * (y - center.y)));

        if (distance + rad <= radius)
            return true;
        return false;
    }

    public bool Intersects(FM_Rectangle rectangle)
    {
        double recX1 = rectangle.GetTopLeft().x;
        double recX2 = rectangle.GetBottomRight().x;
        double recY1 = rectangle.GetTopLeft().y;
        double recY2 = rectangle.GetTopLeft().y;

        // Getting the nearest point of the rectangle to the center of the circle
        double nX = Math.Max(recX1, Math.Min(center.x, recX2));
        double nY = Math.Max(recY1, Math.Min(center.y, recY2));


        // Get the distance between the nearest point and the center
        double distanceX = nX - center.x;
        double distanceY = nY - center.y;
        return (distanceX * distanceX + distanceY * distanceY) <= radius * radius;
    }
}
