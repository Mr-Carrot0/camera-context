using Godot;
using System;

public partial class Player : CharacterBody3D
{
    private enum StateP
    {
        Grounded,
        Crouching,
        Jumping,
        Falling,
        Hanging
    };
    [ExportGroup("Custom")]
    [Export] private StateP PlayerState = StateP.Falling;
    // [Export] private float StretchHeight = 1.2f;
    // [Export] private float SquishHeight = 0.8f;
    // [Export] private Curve EaseCurve;

    [ExportGroup("Physics")]
    [Export] public float SPEED_CROUCH = 2f;
    [Export] public float SPEED_WALK = 5.0f;
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
    [Export] private RayCast3D CrouchRay;
    [Export] private AnimationPlayer Ani;

    private Vector3 lastDir = new(0, 0, 0);
    private float CayoteAccumulator = 0;

    readonly struct STR
    {
        public static readonly StringName crouch = "crouch";
        public static readonly StringName jump = "jump";
    }

    // public override void _Ready()
    // {
    //     // GD.Print(STR.Move_type(1));
    // }

    private void OnJump()
    {
        PlayerState = StateP.Jumping;
        // Vector3 vel = Velocity;
        // vel.Y = JumpVelocity;
        // Velocity = vel;
        Velocity = new Vector3(Velocity.X, JumpVelocity, Velocity.Z);
    }

    private void OnHangTimeEnd()
    {
        PlayerState = StateP.Falling;
    }
    private void HandleMovementZX(ref Vector3 velocity, float delta)
    {
        // get input direction
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");

        // float joyX = Input.GetJoyAxis(0, JoyAxis.LeftX);
        // float joyY = Input.GetJoyAxis(0, JoyAxis.LeftY);

        // joyX = Mathf.Abs(joyX) < 0.01f ? 0 : joyX;
        // joyY = Mathf.Abs(joyY) < 0.01f ? 0 : joyY;

        Vector2 inputDirJoy = new(Input.GetJoyAxis(0, JoyAxis.LeftX), Input.GetJoyAxis(0, JoyAxis.LeftY));
        // GD.PrintS(inputDirJoy, inputDirJoy.LengthSquared());
        if (inputDirJoy.LengthSquared() < 0.003f)
        {
            inputDirJoy = Vector2.Zero;
        }

        if (inputDir == Vector2.Zero) { inputDir = inputDirJoy; }

        Vector3 direction = (Cam.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            lastDir = direction;
        }

        // rotation
        if (lastDir != Vector3.Zero)
        {
            Basis rotBasis = new();

            rotBasis.Z = (lastDir * new Vector3(1, 0, 1)).Normalized();
            rotBasis.X = rotBasis.Z.Cross(Vector3.Down);
            rotBasis.Y = rotBasis.Z.Cross(rotBasis.X);

            Quaternion a = new(Meshes.Basis);
            Quaternion b = new(rotBasis);

            Meshes.Basis = new Basis(a.Slerp(b, 0.1f));
        }

        float sample = -1;
        // Vector2 XZ;


        if (direction != Vector3.Zero)
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

        Vector2 XZ = SpeedCurrent * 50 * delta * sample * new Vector2(lastDir.X, lastDir.Z);
        velocity.X = XZ.X;
        velocity.Z = XZ.Y;

        // ResetTimers();
    }


    // float timer = 0;

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        if (IsOnFloor())
        {
            CayoteAccumulator = 0;

            /** Handle crouch
            * don't uncrouch if there's no room
            * slightly buggy on edgecases
            */
            if (Input.IsActionPressed(STR.crouch) || CrouchRay.IsColliding())
            {
                if (PlayerState != StateP.Crouching)
                {
                    Ani.Play(STR.crouch);
                    PlayerState = StateP.Crouching;
                }
            }
            else
            {
                if (PlayerState == StateP.Crouching && !CrouchRay.IsColliding())
                {
                    Ani.PlayBackwards(STR.crouch);
                    PlayerState = StateP.Grounded;
                }
            }
        }
        else
        {
            CayoteAccumulator += (float)delta;

            if (PlayerState == StateP.Jumping && velocity.Y < 0)
            {
                PlayerState = StateP.Hanging;
                HangTimer.Start();
            }

            if (PlayerState != StateP.Hanging)
            {
                velocity += GetGravity() * (float)delta;
            }
        }

        // Handle Jump
        if (Input.IsActionJustPressed(STR.jump) && PlayerState != StateP.Jumping && (CayoteAccumulator < CAYOTE_TIME || IsOnFloor()))
        {
            CayoteAccumulator = 1; // no double jumps allowed 
            JumpTimer.Start();
            Ani.Play(STR.jump);
            PlayerState = StateP.Jumping;
        }
        SpeedCurrent = Mathf.Lerp(SpeedCurrent, PlayerState != StateP.Crouching ? SPEED_WALK : SPEED_CROUCH, 0.5f);


        HandleMovementZX(ref velocity, (float)delta);

        if (PlayerState == StateP.Falling && IsOnFloor())
        {
            PlayerState = StateP.Grounded;
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
