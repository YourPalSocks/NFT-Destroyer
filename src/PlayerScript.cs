using Godot;
using System;

public class PlayerScript : Spatial
{
	[Export]
	private PackedScene match_projectile;
	[Export]
	private float match_cooldown = 15;
	private float cur_cooldown = 0;

	public override void _Process(float delta)
	{
		if(cur_cooldown <= 0 && GameManager.can_throw_matches)
		{
			if(Input.IsActionJustPressed("player_action1"))
			{
				spawnMatch();
			}
		}
		else
		{
			cur_cooldown -= 0.1f;
		}
	}

	private void spawnMatch()
	{
		Camera main_cam = GetViewport().GetCamera();
		RigidBody new_match = (RigidBody) match_projectile.Instance();
		GetParent().AddChild(new_match);
		new_match.Translation = GlobalTransform.origin + -main_cam.Transform.basis.z;
		new_match.Rotation = main_cam.Rotation;
		new_match.ApplyCentralImpulse(-main_cam.Transform.basis.z * 4);
		cur_cooldown = match_cooldown;
	}

	public void reduceMatchTime(float amt)
	{
		match_cooldown -= amt;
	}
}
