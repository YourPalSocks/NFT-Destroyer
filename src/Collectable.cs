using Godot;
using System;

public class Collectable : RigidBody
{
    private GameManager manager;
    private AudioStreamPlayer audio;
    private bool in_coin = false;
    private bool collected = false;

    public override void _Ready()
    {
        audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        manager = GetParent<GameManager>();
    }

    public override void _Process(float delta)
    {
        if(in_coin && !collected)
        {
            if(Input.IsActionJustPressed("player_action1"))
            {
                audio.Play();
                GameManager.can_throw_matches = true;
                collected = true;
                Visible = false;
                manager.addCoin();
            }
        }

        // Remove after sound is done
        if(collected && audio.Playing == false)
        {
            QueueFree();
        }
    }

    private void _on_Area_mouse_entered()
    {
        in_coin = true;
        GameManager.can_throw_matches = false;
    }

    private void _on_Area_mouse_exited()
    {
        in_coin = false;
        GameManager.can_throw_matches = true;
    }

    private void _on_VisibilityNotifier_screen_exited()
    {
        QueueFree();
    }
}
