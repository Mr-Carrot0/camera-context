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
    [Export] Node3D Player;
    [Export] Camera3D Camera;
    [ExportSubgroup("CameraHierachy")]
    [Export] Node3D CamRotX;
    [Export] Node3D CamRotY;
    [Export] Node3D CamZoom;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GlobalPosition = Player.GlobalPosition;
        Camera.GlobalPosition = CamZoom.GlobalPosition;
        Camera.GlobalRotation = CamZoom.GlobalRotation;
    }

    public override void _PhysicsProcess(double delta)
    {
        DebugDraw3D.DrawSphere(GlobalPosition);
        GlobalPosition = GlobalPosition.Slerp(Player.GlobalPosition, Weight);
        Camera.GlobalPosition = Camera.GlobalPosition.Slerp(CamZoom.GlobalPosition, Weight);
        Camera.GlobalRotation = Camera.GlobalRotation.Slerp(CamZoom.GlobalRotation, Weight);
    }

    public override void _Process(double delta)
    {
        Vector2 camDelta = GetJoy() * 0.05f;

        CamRotY.RotateY(camDelta.X);

        // float rx = CamRotX.Rotation.X;
        // rx = Mathf.Clamp(rx + camDir.Y, -1f, 1f);
        // CamRotX.RotateX(camDir.Y);
        CamRotX.Rotation = new(Mathf.Clamp(CamRotX.Rotation.X + camDelta.Y, -1f, 1f), 0, 0);

        // Camera.GlobalRotation = Camera.GlobalRotation.Slerp(CamRotZ.GlobalRotation, Weight);
        Camera.GlobalRotation = new(
            Mathf.LerpAngle(Camera.GlobalRotation.X, CamZoom.GlobalRotation.X, Weight),
            Mathf.LerpAngle(Camera.GlobalRotation.Y, CamZoom.GlobalRotation.Y, Weight),
            Camera.GlobalRotation.Z);
        // Camera.GlobalRotation = Camera.GlobalRotation.Slerp(CamZoom.GlobalRotation, Weight);
    }
}
