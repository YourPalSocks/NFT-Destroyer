using Godot;
using System;

public class NFT : Area
{
	private CPUParticles fire;
	private AudioStreamPlayer sound;
	private AudioStreamPlayer flip;

	[Export]
	private float alive_time = 5f;
	private float cur_time;
	private AnimationPlayer anim;
	private float burn_size = 0f;

	private ShaderMaterial mat;

	public override void _Ready()
	{
		mat = (ShaderMaterial) GetChild<CSGMesh>(0).Material;
		fire = GetNode<CPUParticles>("FireParticles");
		sound = GetNode<AudioStreamPlayer>("Audio");
		flip = GetNode<AudioStreamPlayer>("FlipSFX");
		anim = GetNode<AnimationPlayer>("AnimationPlayer");
		cur_time = alive_time;
		mat.SetShaderParam("dissolve_amount", 0);

		// Animation and Sound
		anim.Play("nft_drop");
		flip.Play();
	}

	public override void _Process(float delta)
	{
		if(fire.Emitting)
		{
			if(cur_time > 0)
			{
				cur_time -= 0.1f;
				burn_size += 0.015f;
				// Change burn size
				mat.SetShaderParam("dissolve_amount", burn_size);
			}
			else
			{
				QueueFree();
			}
		}
	}

	private void _on_NFT_body_entered(Node body)
	{
		if(body.GetType() == typeof(MatchScript))
		{
			fire.Emitting = true;
			sound.Play();
		}
	}

	/// <summary> Replaces the nft's material with <paramref name="newMat"/>. </summary>
	/// <param name="newMat">The new material to replace the current one.</param>
	public void replaceTexture(Material newMat)
	{
		mat.SetShaderParam("texture_albedo", ((SpatialMaterial)newMat).AlbedoTexture);
	}
}
