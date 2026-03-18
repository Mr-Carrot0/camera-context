using Godot;
using System;

public partial class CameraController : Node3D
{
    public static Vector2 GetJoy(bool right = true, int device = 0)
    {
        Vector2 ret = right switch
        {
            true => new(Input.GetJoyAxis(device, JoyAxis.RightX), Input.GetJoyAxis(device, JoyAxis.RightY)),
            false => new(Input.GetJoyAxis(device, JoyAxis.LeftX), Input.GetJoyAxis(device, JoyAxis.LeftY)),
        };

        if (ret.LengthSquared() < 0.003f)
        {
            ret = Vector2.Zero;
        }

        return ret;
    }
    [Export] float Weight = 0.2f;
    [ExportGroup("Nodes")]
    [Export] Player Player;
    Vector3 Target;
    [Export] Camera3D Camera;
    [ExportSubgroup("CameraHierachy")]
    [Export] Node3D CamRotX;
    [Export] Node3D CameraYaw;
    [Export] Node3D CamZoom;
    public override void _Ready()
    {
        GlobalPosition = Player.GlobalPosition;
        Target = Target.Lerp(Player.GlobalPosition, 0.1f);
        Camera.GlobalPosition = CamZoom.GlobalPosition;
        Camera.GlobalRotation = CamZoom.GlobalRotation;
    }

    public override void _PhysicsProcess(double delta)
    {
        Target = Player.GlobalPosition + Player.Velocity;

        GlobalPosition = GlobalPosition.Lerp(
            Player.GlobalPosition
            .Lerp(Target, Weight)
            , Weight);

        Camera.GlobalPosition = Camera.GlobalPosition.Lerp(CamZoom.GlobalPosition, Weight);
        Camera.GlobalRotation = Camera.GlobalRotation.Slerp(CamZoom.GlobalRotation, Weight);

        DebugDraw3D.DrawSphere(GlobalPosition);
    }

    public override void _Process(double delta)
    {
        Vector2 camDelta = GetJoy() * 1.5f * (float)delta;

        CameraYaw.RotateY(camDelta.X);

        // float rx = CamRotX.Rotation.X;
        // rx = Mathf.Clamp(rx + camDir.Y, -1f, 1f);
        // CamRotX.RotateX(camDir.Y);
        CamRotX.Rotation = new(Mathf.Clamp(CamRotX.Rotation.X + camDelta.Y, -1.2f, 1f), 0, 0);

        Camera.LookAt(Player.GlobalPosition);
        // Camera.GlobalRotation = Camera.GlobalRotation.Slerp(CamRotZ.GlobalRotation, Weight);
        // GD.Print(Camera.GlobalRotation);
        // .GlobalRotation = new(
        //     Mathf.LerpAngle(Camera.GlobalRotation.X, CamZoom.GlobalRotation.X, Weight),
        //     Mathf.LerpAngle(Camera.GlobalRotation.Y, CamZoom.GlobalRotation.Y, Weight),
        //     0);
        // Camera.GlobalRotation = Camera.GlobalRotation.Slerp(CamZoom.GlobalRotation, Weight);
    }
}
