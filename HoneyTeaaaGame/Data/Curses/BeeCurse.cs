using Godot;
using System;

[GlobalClass]
public partial class BeeCurse : Curse
{
	public override float SetGemPowerMult(Gem theGem) {
		if(theGem.color == "Red") return 0f;
		return 1f;
	}
}
