using Godot;
using System;
public static class Utils
{
    static Utils()
    {
        World3D _tmp = World = Main.Instance.GetWorld3D();
        SpaceState = _tmp.DirectSpaceState;
    }
    public static readonly World3D World;
    public static readonly PhysicsDirectSpaceState3D SpaceState;

    public static class RayKeys
    {
        public static readonly StringName Collider = "collider";      // CollisionObject3D
        public static readonly StringName ColliderId = "collider_id"; // int
        public static readonly StringName Normal = "normal";          // Vector3
        public static readonly StringName Position = "position";      // Vector3
        public static readonly StringName FaceIndex = "face_index";   // int
        public static readonly StringName RID = "rid";                // int
        public static readonly StringName Shape = "shape";            // int
    }
    public struct RayHit
    {
        public Vector3 Position;
        public CollisionObject3D Collider;
        public uint? ColliderId;
    }
    public static readonly Godot.Collections.Array<Rid> RayExlude = [Main.P1.GetRid()];
    public static Vector3? RayCast(Vector3 from, Vector3 to)
    {
        Godot.Collections.Dictionary result = SpaceState.IntersectRay(
            PhysicsRayQueryParameters3D.Create(from, to, exclude: RayExlude)
            );

        if (result.Count == 0) return null;

        if (result.TryGetValue(RayKeys.Position, out Variant pos))
            return (Vector3)pos;

        return null;
    }
    public enum Vec3Axies { X, Y, Z }
    public static Vector2 GetJoy(bool leftStick = false, int device = 0)
    {
        Vector2 ret = new(
            Input.GetJoyAxis(device, JoyAxis.RightX),
            Input.GetJoyAxis(device, leftStick ? JoyAxis.LeftY : JoyAxis.RightY)
            );

        if (ret.LengthSquared() < 0.003f)
        {
            return Vector2.Zero;
        }

        return ret;
    }
    public static Vector3 SetAxies(float value, Vec3Axies axies, Vector3 origin)
    {
        switch (axies)
        {
            case Vec3Axies.X:
                origin.X = value;
                break;
            case Vec3Axies.Y:
                origin.Y = value;
                break;
            case Vec3Axies.Z:
                origin.Z = value;
                break;
        }
        return origin;
    }
    public static void Throttle(ref float timer, float waitTime, float dt, Action<float> fn)
    {
        if (timer > waitTime)
        {
            timer = 0;
            fn(dt);
        }
        else { timer += dt; }
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