using Godot;
using System;

[GlobalClass]
public partial class SweetCurse : Curse
{
	[Export] public float weakMultiply;
	[Export] public float strongMultiply;
	public override float SetGemPowerMult(Gem theGem) {
		if(theGem is RedGem) return weakMultiply;
		if(theGem is BlueGem || theGem is GreenGem) return strongMultiply;
		return 1f;
	}
}
