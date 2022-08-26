using Godot;
using System;

public class GPUScript : Spatial
{
    // Components
    private AnimationPlayer anim;
    private Timer timer;

    [Signal]
    delegate void gpu_timeout();

    public override void _Ready()
    {
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        timer = GetNode<Timer>("Timer");
    }

    public override void _Process(float delta)
    {
        if(Visible && timer.IsStopped())
        {
            timer.Start();
        }
    }

    private void _on_Timer_timeout()
    {
        // Add coin to GameManager
        anim.Play("Plus One");
        timer.Start(5);
        EmitSignal("gpu_timeout");
    }
}
