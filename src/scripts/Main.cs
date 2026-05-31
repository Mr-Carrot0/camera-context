using Godot;
using System;

public sealed partial class Main : Node3D
{
    public static Main Instance { get; private set;}
    Main()
    {
        if (Instance != null) throw new Exception("Main: singleton already instaciated");
        Instance = this;
    }
    [Export] private Player _P1;
    [Export] private CameraController _CamControl;
    [Export] private Camera3D _Cam3D;
    public static Camera3D Cam3D => Instance._Cam3D;
    public static Player P1 => Instance._P1;
    public static CameraController CamControl => Instance._CamControl;
}
