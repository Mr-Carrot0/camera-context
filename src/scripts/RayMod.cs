using Godot;
using System;

public partial class RayMod : Node3D
{
    [Export(PropertyHint.Range, "0,20")] uint rayCount;

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            uint childCount = (uint)GetChildCount();
            uint diff = rayCount - childCount;
            if (diff != 0)
            {
                if (diff > 0)
                {

                }
                else
                {
                    // diff < 0
                }
                
            }
        }
        else
        {
            // in game only
        }
    }

}
