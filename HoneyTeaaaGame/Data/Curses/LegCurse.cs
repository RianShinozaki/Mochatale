using Godot;
using System;

[GlobalClass]
public partial class LegCurse : Curse
{
	
	[Export] public float strongMultiply = 2;
	public override float SetGemCostMult(Gem theGem) {
		if(Battle.Instance.currentGemIndex == Battle.Instance.totalGems-1) return strongMultiply;
		return 1f;
	}
}
