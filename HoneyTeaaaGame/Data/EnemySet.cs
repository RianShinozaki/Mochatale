using Godot;
using System;

[GlobalClass]
public partial class EnemySet : Resource
{
	[Export] public Godot.Collections.Array<PackedScene> enemies;
	[Export] public PackedScene battleBG;
	[Export] public AudioStream battleMusic;
	[Export] public float battleMusicVolume;
}
