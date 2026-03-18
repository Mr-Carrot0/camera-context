using Godot;
using System;

public static class Utils
{
    public static Vector2 FlattenVecXZ(Vector3 vec)
    {
        return new(vec.X, vec.Z);
    }
    public static Vector2 FlattenVecXY(Vector3 vec)
    {
        return new(vec.X, vec.Y);
    }
    public static Vector3 UnFlattenVecXZ(Vector2 vec)
    {
        return new(vec.X, 0, vec.Y);
    }
    public static Vector3 UnFlattenVecXY(Vector2 vec)
    {
        return new(vec.X, vec.Y, 0);
    }
}