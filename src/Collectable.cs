using Godot;
using System;

public class Collectable : RigidBody
{
	private GameManager manager;
	private AudioStreamPlayer audio;
	private bool in_coin = false;
	private bool collected = false;
	private int coinAmt = 1;

	private AnimationPlayer anim;

	public override void _Ready()
	{
		audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		manager = GetParent<GameManager>();
		anim = GetNode<AnimationPlayer>("AnimationPlayer");
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
				GetChild<Spatial>(0).Visible = false;
				GravityScale = 0;
				manager.addCoin(coinAmt);
				// Play animation here
				anim.Play("Plus One");
			}
		}

		// Remove after sound is done
		if(collected && !anim.IsPlaying())
		{
			QueueFree();
		}
	}

	public void setCoinAmt(int amt)
	{
		coinAmt = amt;
		GetNode<Label3D>("Text").Text = "+" + coinAmt.ToString();
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
