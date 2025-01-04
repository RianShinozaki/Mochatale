using Godot;
using System;

[GlobalClass]
public partial class SweetCurse : Curse
{
	
	[Export] public float weakMultiply = 0.5f;
	[Export] public float strongMultiply = 2;
	public override float SetGemPowerMult(Gem theGem) {
		if(theGem is AttackGem) return weakMultiply;
		if(theGem is BlueGem || theGem is GreenGem) return strongMultiply;
		return 1f;
	}
}
