using Godot;
using System;

public static class Utils
{
    public enum VectorAxies { X, Y, Z }
    public static Vector2 GetJoy(bool leftStick = false, int device = 0)
    {
        Vector2 ret = new(
            Input.GetJoyAxis(device, JoyAxis.RightX),
            Input.GetJoyAxis(device, leftStick ? JoyAxis.LeftY : JoyAxis.RightY)
            );

        if (ret.LengthSquared() < 0.003f)
        {
            ret = Vector2.Zero;
        }

        return ret;
    }
    public static Vector3 SetAxies(float value, VectorAxies axies,Vector3 origin)
    {
        switch (axies)
        {
            case VectorAxies.X:
                origin.X = value;
                break;
            case VectorAxies.Y:
                origin.Y = value;
                break;
            case VectorAxies.Z:
                origin.Z = value;
                break;
        }
        return origin;
    }
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