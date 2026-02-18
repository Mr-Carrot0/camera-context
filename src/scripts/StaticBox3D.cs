using Godot;
using System;

public partial class StaticBox3D : StaticBody3D
{
    // Called when the node enters the scene tree for the first time.
    // [Export] public CollisionShape3D Shape;
    public override void _Ready()
    {
        MeshInstance3D MeshInst = GetNode<MeshInstance3D>("Mesh");
        CollisionShape3D ColShape = GetNode<CollisionShape3D>("Shape");

        BoxShape3D Shape = new()
        {
            Size = ((BoxMesh)MeshInst.Mesh).Size
        };

        ColShape.Shape = Shape;
    }
}
