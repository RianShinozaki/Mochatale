using Godot;
using System;

public partial class OverworldData : Node2D
{
	public static OverworldData Instance;
	[Export] public AudioStream overworldTrack;
	public float playbackOffset;
	public override void _Ready()
	{
		MusicController.StartMusic(overworldTrack, 1f);
		Instance = this;
	}

}
