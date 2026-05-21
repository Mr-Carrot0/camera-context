using Godot;
using System;

public partial class CameraController : Node3D
{
    public static Vector2 GetJoy(bool leftStick = false, int device = 0)
    {
        Vector2 ret = leftStick switch
        {
            true => new(Input.GetJoyAxis(device, JoyAxis.LeftX), Input.GetJoyAxis(device, JoyAxis.LeftY)),
            false => new(Input.GetJoyAxis(device, JoyAxis.RightX), Input.GetJoyAxis(device, JoyAxis.RightY)),
        };

        if (ret.LengthSquared() < 0.003f)
        {
            ret = Vector2.Zero;
        }

        return ret;
    }
    [Export] float Weight = 0.2f;
    // [ExportGroup("Nodes")]
    [Export] Player Player;
    Vector3 Target;
    short _InvertY; // 1|-1
    [Export]
    bool InvertY
    {
        set
        {
            _InvertY = value switch
            {
                true => -1,
                false => 1,
            };
        }
        get
        {
            return _InvertY switch
            {
                1 => false,
                -1 => true,
            };
        }
    }
    [Export] Camera3D Camera;
    [ExportGroup("CameraHierachy")]
    [Export] Node3D CamPitch;
    [Export] Node3D CameraYaw;
    [Export] Node3D CamZoom;
    Vector3 MaxZoom;

    public override void _Ready()
    {
        MaxZoom = CamZoom.Position;
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
        Vector2 camDelta = GetJoy() * 2f * (float)delta;

        CameraYaw.RotateY(_InvertY * camDelta.X);

        // float rx = CamRotX.Rotation.X;
        // rx = Mathf.Clamp(rx + camDir.Y, -1f, 1f);
        // CamRotX.RotateX(camDir.Y);
        CamPitch.Rotation = new(Mathf.Clamp(CamPitch.Rotation.X + camDelta.Y, -1.2f, 1f), 0, 0);

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
