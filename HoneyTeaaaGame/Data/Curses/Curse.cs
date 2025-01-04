using Godot;
using System;

[GlobalClass]
public partial class Curse : Resource
{
	[Export] public string tag = "You've been hit with a witchy curse!";
	public virtual bool CanGemTrigger(Gem theGem) {
		return true;
	}
	public virtual float SetGemCostMult(Gem theGem) {
		return 1;
	}
	public virtual float SetGemPowerMult(Gem theGem) {
		return 1;
	}
}
