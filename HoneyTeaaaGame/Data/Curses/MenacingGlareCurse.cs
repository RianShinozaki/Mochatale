using Godot;
using System;

[GlobalClass]
public partial class MenacingGlareCurse : Curse
{
	public override bool CanGemTrigger() {
		RandomNumberGenerator rand = new RandomNumberGenerator();
		int i = rand.RandiRange(1,3);
		GD.Print(i);
		return i != 1;
	}
}
