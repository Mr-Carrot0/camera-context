using Godot;
using System;
using Axies = Utils.Vec3Axies;
using Draw = DebugDraw3D;
public partial class CameraController : Node3D
{
    enum InvertYEnum : sbyte { NORMAL = 1, INVERTED = -1 }
    [Export] Player player;
    [Export] Camera3D Camera;
    [Export] public bool Enabled = true;
    [Export] float Weight = 0.2f;
    [Export] float CameraStrength = 1f;
    // TODO: better interpelation, probably w/ Curve
    [Export] float waitForAuto = 80f;
    [Export] Curve waitcurve;
    float timeSinceLastCamInput;
    Vector3 Target;
    [Export] private InvertYEnum _InvertY = InvertYEnum.NORMAL;
    sbyte InvertY { get { return (sbyte)_InvertY; } }

    [ExportGroup("CameraHierachy")]

    /// <summary>
    /// rotates when moving stick in X (around the Y-axies)
    /// </summary>
    [Export] Node3D CamYaw;

    /// <summary>
    /// rotates when moving stick in Y (around X-axies(?))
    /// </summary>
    [Export] Node3D CamPitch;

    /// <summary>
    /// End of node tree, final transform
    /// </summary>
    [Export] Node3D CamZoom;
    private Vector3 max_zoom;
    Vector3 MaxZoom
    {
        get { return max_zoom; }
        set
        {
            if (max_zoom != Vector3.Zero) throw new Exception("CameraController.MaxZoom Reasigment not allowed");
            max_zoom = value;
        }
    }
    // struct VecRot2(Func<float, float> x, Func<float, float> y)
    // {
    //     public readonly float GetX(float dt) { return x(dt); }
    //     public readonly float GetY(float dt) { return y(dt); }
    // }

    // Raycasting
    [Export(PropertyHint.Range, "0,20")] uint rayCount = 8;
    [Export] float rayDelay = 2f;
    float rayTimer = 0;


    public override void _Ready()
    {
        // VecRot2 TargetRot = new(
        //     delegate (float delta)
        //     {
        //         return Mathf.LerpAngle(CamPitch.GlobalRotation.X, -0.3f, Weight * (float)delta);
        //     },
        //     delegate ()
        //     {

        //     }
        // );

        for (uint i = 0; i < rayCount; i++)
        {
            float angle = (float)i / rayCount * Mathf.Tau;
            GD.PrintS(i, angle);
        }
        GD.Print("/test");

        MaxZoom = CamZoom.Position;
        GlobalPosition = player.GlobalPosition + Vector3.Up;
        Target = player.GlobalPosition + Vector3.Up;
        Camera.GlobalPosition = CamZoom.GlobalPosition;
        // Camera.GlobalRotation = CamZoom.GlobalRotation;
        // var m = delegate (int dt) { return dt * 3f; };
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!Enabled) return;
        Target = player.GlobalPosition + player.Velocity * 0.5f + Vector3.Up;

        GlobalPosition = GlobalPosition.Lerp(
            player.GlobalPosition
            .Lerp(Target, Weight)
            , Weight);

        Camera.GlobalPosition = Camera.GlobalPosition.Lerp(CamZoom.GlobalPosition, Weight);
        Camera.GlobalRotation = Camera.GlobalRotation.Slerp(CamZoom.GlobalRotation, Weight);

        Draw.DrawSphere(GlobalPosition);
        Vector2 rightStick = Utils.GetJoy();

        if (rightStick.IsEqualApprox(Vector2.Zero))
        {
            if (timeSinceLastCamInput < waitForAuto)
            {
                timeSinceLastCamInput++;
            }
            else
            {
                // GD.Print("autocam: ", timeSinceLastCamInput);
                // if (CamPitch.Rotation.DistanceSquaredTo(Vector3.Zero) > 0.003f)
                CamPitch.GlobalRotation = Utils.SetAxies(Mathf.LerpAngle(CamPitch.GlobalRotation.X, -0.3f, Weight * (float)delta),
                    Axies.X, CamPitch.GlobalRotation);

                // if (CamPitch.Rotation.DistanceSquaredTo(Vector3.Zero) > 0.003f)
                CamYaw.GlobalRotation = Utils.SetAxies(Mathf.LerpAngle(CamYaw.GlobalRotation.Y, Mathf.Pi + player.GlobalRotation.Y, Weight * (float)delta),
                    Axies.Y, CamYaw.GlobalRotation);
                Mathf.LerpAngle(CamYaw.GlobalRotation.Y, 0f, Weight /* * (float)delta */);
            }
        }
        else
        {
            timeSinceLastCamInput = 0;
            Vector2 camDelta = rightStick * CameraStrength * (float)delta;

            CamYaw.RotateY(InvertY * camDelta.X);

            // float rx = CamRotX.Rotation.X;
            // rx = Mathf.Clamp(rx + camDir.Y, -1f, 1f);
            // CamRotX.RotateX(camDir.Y);
            CamPitch.Rotation = new(Mathf.Clamp(CamPitch.Rotation.X + camDelta.Y, -1.2f, 1f), 0, 0);
        }

        Camera.LookAt(player.GlobalPosition);

        /// Raycasting
        /// 
        // if (rayTimer+=(float)delta > rayDelay)
        // {
        // rayTimer = 0;

        for (uint i = 0; i < rayCount; i++)
        {
            float angle = (float)i / rayCount * Mathf.Tau;
            Vector3 diff = player.GlobalPosition - CamZoom.GlobalPosition;

            float dist = player.GlobalPosition.DistanceTo(Camera.GlobalPosition);


            Vector2 screenPt = Camera.UnprojectPosition(player.GlobalPosition) + (Vector2.Right).Rotated(angle);



            Vector3 diffNorm = diff.Normalized();

            Utils.Throttle(ref rayTimer, rayDelay, (float)delta, _ => GD.PrintS(diff, diffNorm));

            

            Vector3 offset = Vector3.Up.Cross(diffNorm).Rotated(diffNorm, angle);
            // Vector3 offset = Camera.ProjectPosition(screenPt,dist);

            Draw.DrawSphere(player.GlobalPosition + offset, 0.1f, Colors.Red);
            // GD.PrintS(i, angle);
            // GD.Print(Target);

        }
        // }
    }

    private void SetAngle(Vector2 rot)
    {
        CamPitch.GlobalRotation = Utils.SetAxies(rot.X, Axies.X, CamPitch.GlobalRotation);
        CamYaw.GlobalRotation = Utils.SetAxies(rot.Y, Axies.X, CamYaw.GlobalRotation);
    }



}
