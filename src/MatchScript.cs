using Godot;
using System;

public class MatchScript : RigidBody
{
    [Export]
    private float destroyTime = 10f;

    public override void _Process(float delta)
    {
        if(destroyTime > 0)
            destroyTime -= 0.5f;
        else
            this.QueueFree();
    }

}
 