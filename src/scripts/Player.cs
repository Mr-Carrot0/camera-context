using Godot;
using System;

public partial class Player : CharacterBody3D
{
    private enum State
    {
        Grounded,
        Jumping,
        Falling,
        Hanging
    };
    [ExportGroup("Custom")]
    [Export] private State PlayerState = State.Falling;

    [ExportGroup("Physics")]
    [Export] public float SPEED_MAX = 5.0f;
    [Export] private float CAYOTE_TIME = 0.12f;
    public float SpeedCurrent = 3.0f;
    [Export] public float JumpVelocity = 6f;
    [Export] public Curve curve_easing;
    [Export] public Curve curve_linear;

    private float TimerAcceleration = 0;
    private float TimerDeceleration = 0;

    private void ResetTimers()
    {
        if (TimerAcceleration >= 1) TimerAcceleration = 0;
        if (TimerDeceleration >= 1) TimerDeceleration = 0;
    }

    [ExportGroup("Nodes")]
    [Export] private Camera3D Cam;
    [Export] private Node3D Meshes;
    [Export] private Timer JumpTimer;
    [Export] private Timer HangTimer;
    [Export] private AnimationPlayer Ani;

    private Vector3 lastDir = new(0, 0, 0);
    private float CayoteAccumulator = 0;
    // public Vector3 Direction;
    readonly struct STR
    {
        public static readonly StringName jump = "jump";
    }

    private void OnJump()
    {
        PlayerState = State.Jumping;
        // Vector3 vel = Velocity;
        // vel.Y = JumpVelocity;
        // Velocity = vel;
        Velocity = new Vector3(Velocity.X, JumpVelocity, Velocity.Z);
    }

    private void OnHangTimeEnd()
    {
        PlayerState = State.Falling;
    }
    private Vector2 HandleMovementZX(float delta)
    {
        // get input direction
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");

        Vector2 inputDirJoy = new(Input.GetJoyAxis(0, JoyAxis.LeftX), Input.GetJoyAxis(0, JoyAxis.LeftY));
        // GD.PrintS(inputDirJoy, inputDirJoy.LengthSquared());
        if (inputDirJoy.LengthSquared() < 0.003f)
        {
            inputDirJoy = Vector2.Zero;
        }

        if (inputDir == Vector2.Zero) { inputDir = inputDirJoy; }

        Vector3 Direction = (Cam.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        // GD.Print(Direction.Length());

        if (Direction != Vector3.Zero)
        {
            lastDir = Direction;
        }

        // rotation
        if (lastDir != Vector3.Zero)
        {
            Basis rotBasis = new();

            rotBasis.Z = (lastDir * new Vector3(1, 0, 1)).Normalized();
            rotBasis.X = Vector3.Up.Cross(rotBasis.Z);
            rotBasis.Y = rotBasis.Z.Cross(rotBasis.X);

            Quaternion a = new(Meshes.Basis);
            Quaternion b = new(rotBasis);

            Meshes.Basis = new Basis(a.Slerp(b, 0.1f));
        }

        float sample;

        if (Direction != Vector3.Zero)
        {
            TimerDeceleration = 0;
            TimerAcceleration += delta;
            sample = curve_easing.Sample(TimerAcceleration);
        }
        else
        {
            TimerAcceleration = 0;
            TimerDeceleration += delta * 6f;
            sample = 1 - curve_linear.Sample(TimerDeceleration);
        }

        return 70 * SpeedCurrent * delta * sample * Utils.FlattenVecXZ(new Vector3(Direction.X, 0, Direction.Z).Normalized());
    }


    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("restart"))
        {
            GetTree().ReloadCurrentScene();
            return;
        }
        Vector3 velocity = Velocity;
        if (IsOnFloor())
        {
            CayoteAccumulator = 0;
        }
        else
        {
            CayoteAccumulator += (float)delta;

            if (PlayerState == State.Jumping && velocity.Y < 0)
            {
                PlayerState = State.Hanging;
                HangTimer.Start();
            }

            if (PlayerState != State.Hanging)
            {
                velocity += GetGravity() * (float)delta;
            }
        }

        // Handle Jump
        if (Input.IsActionJustPressed(STR.jump) && PlayerState != State.Jumping && (CayoteAccumulator < CAYOTE_TIME || IsOnFloor()))
        {
            CayoteAccumulator = 1; // no double jumps allowed 
            JumpTimer.Start();
            Ani.Play(STR.jump);
            PlayerState = State.Jumping;
        }
        SpeedCurrent = Mathf.Lerp(SpeedCurrent, SPEED_MAX, 0.5f);


        Vector2 XZ = HandleMovementZX((float)delta);
        velocity.X = XZ.X;
        velocity.Z = XZ.Y;

        if (PlayerState == State.Falling && IsOnFloor())
        {
            PlayerState = State.Grounded;
        }

        Velocity = velocity;
        MoveAndSlide();
        // GD.Print(Velocity.Length());
    }
}
