using Godot;
using System;
using Axies = Utils.VectorAxies;
public partial class CameraController : Node3D
{
    [Export] public bool Enabled = true;
    enum InvertYEnum : sbyte { NORMAL = 1, INVERTED = -1 }
    [Export] float Weight = 0.2f;
    [Export] float CameraStrength = 2f;
    [Export] Player Player;
    [Export] Camera3D Camera;
    Vector3 Target;
    [Export] InvertYEnum _InvertY = InvertYEnum.NORMAL;// 1|-1
    sbyte InvertY { get { return (sbyte)_InvertY; } }

    [ExportGroup("CameraHierachy")]
    /// <summary>
    /// rotates when moving stick in X (around the Y-axies)
    /// </summary>
    [Export] Node3D CameraYaw;
    /// <summary>
    /// rotates when moving stick in Y (around X-axies(?))
    /// </summary>
    [Export] Node3D CamPitch;
    /// <summary>
    /// End of node tree, final transform
    /// </summary>
    [Export] Node3D CamZoom;
    Vector3 MaxZoom;
    float timeSinceLastCamInput;

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
        if (!Enabled) return;
        Target = Player.GlobalPosition + Player.Velocity * 0.5f;

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
        if (!Enabled) return;
        Vector2 joyStick = Utils.GetJoy();
        if (joyStick == Vector2.Zero)
        {
            if (timeSinceLastCamInput < 300f)
            {
                timeSinceLastCamInput++;
            }
            else
            {
                GD.Print("autocam: ",timeSinceLastCamInput);
                CamPitch.Rotation = Utils.SetAxies(Mathf.LerpAngle(CamPitch.Rotation.X, 0f, Weight * (float)delta),
                    Axies.X, CamPitch.Rotation);
                CameraYaw.Rotation = Utils.SetAxies(Mathf.LerpAngle(CameraYaw.Rotation.Y, 0f, Weight * (float)delta),
                    Axies.Y, CameraYaw.Rotation);
                Mathf.LerpAngle(CameraYaw.Rotation.Y, 0f, Weight /* * (float)delta */);
            }
        }
        else
        {
            timeSinceLastCamInput = 0;
            Vector2 camDelta = joyStick * CameraStrength * (float)delta;

            CameraYaw.RotateY(InvertY * camDelta.X);

            // float rx = CamRotX.Rotation.X;
            // rx = Mathf.Clamp(rx + camDir.Y, -1f, 1f);
            // CamRotX.RotateX(camDir.Y);
            CamPitch.Rotation = new(Mathf.Clamp(CamPitch.Rotation.X + camDelta.Y, -1.2f, 1f), 0, 0);
        }

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
