using Godot;
using System;

public partial class Curse : Resource
{
	[Export] public string tag = "You've been hit with a witchy curse!";
	public virtual bool CanGemTrigger() {
		return true;
	}
}
