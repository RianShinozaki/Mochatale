using Godot;
using System;


[GlobalClass]
public partial class PlayerLevelData : Resource {
    [Export] public float expNeeded;
	[Export] public float maxHP;
	[Export] public float maxMP;
	[Export] public int handSize;
}