using Godot;
using System;

public partial class BlueGem : Gem
{
	[Export] public float magicHeal = 4;
	
    public async override void Trigger()
    {
		//Play the animation
		GemPowerAnimation(Colors.Cyan, magicHeal, Battle.Instance.player.GlobalPosition, false);
		await ToSignal(this, "AnimationFinished");

		Battle.Instance.player.ChangeMagic(magicHeal * GetMult());

		GetParent<Node>().RemoveChild(this);
		QueueFree();
		EmitSignal(SignalName.FinishedTrigger);
    }
	public override string GetDescription() {
		string desc = description;
		desc = desc.Replace("[/0]", GetMult().ToString("0.00"));
		desc = desc.Replace("[/1]", (magicHeal * GetMult()).ToString("0.00"));
		return desc;
	}
}
