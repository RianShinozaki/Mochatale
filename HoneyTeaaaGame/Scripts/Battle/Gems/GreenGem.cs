using Godot;
using System;

public partial class GreenGem : Gem
{
	[Export] public float damage = 4;
	[Export] public float cost = 4;
	
	public AudioStream healSound;
    public override void _Ready()
    {
        base._Ready();
		healSound = GD.Load<AudioStream>("res://Audio/Sounds/037 Heal.wav");

    }

    public async override void Trigger()
    {
		//Play the animation
		GemPowerAnimation(Colors.LimeGreen, damage, Battle.Instance.player.GlobalPosition, cost);
		await ToSignal(this, "AnimationFinished");

		//Heal player
		Battle.Instance.player.ChangeHP(damage * GetPowerMult());
		SFXController.PlaySound(healSound);

		GetParent<Node>().RemoveChild(this);
		QueueFree();
		EmitSignal(SignalName.FinishedTrigger);
    }
	public override string GetDescription() {
		string desc = description;
		desc = desc.Replace("[/0]", GetPowerMult().ToString("0.00"));
		desc = desc.Replace("[/1]", (damage * GetPowerMult()).ToString("0.00"));
		desc = desc.Replace("[/2]", (cost * GetPowerMult()).ToString("0.00"));
		return desc;
	}
}
