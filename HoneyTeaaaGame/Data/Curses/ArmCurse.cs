using Godot;
using System;

[GlobalClass]
public partial class ArmCurse : Curse
{
	
	[Export] public float strongMultiply = 2;
	public override float SetGemCostMult(Gem theGem) {
		if(Battle.Instance.currentGemIndex == 0) return strongMultiply;
		return 1f;
	}---
}
