using Godot;
using System;

public partial class BlueGem : Gem
{
	[Export] public float magicHeal = 4;
	public AudioStream healSound;
    public override void _Ready()
    {
        base._Ready();
		healSound = GD.Load<AudioStream>("res://Audio/Sounds/028 Pray.wav");
    }


    public async override void Trigger()
    {
		//Play the animation
		GemPowerAnimation(Colors.Cyan, magicHeal, Battle.Instance.player.GlobalPosition, 0, false);
		await ToSignal(this, "AnimationFinished");

		Battle.Instance.player.ChangeMagic(magicHeal * GetPowerMult());
		SFXController.PlaySound(healSound);

		GetParent<Node>().RemoveChild(this);
		QueueFree();
		EmitSignal(SignalName.FinishedTrigger);
    }
	public override string GetDescription() {
		string desc = description;
		desc = desc.Replace("[/0]", GetPowerMult().ToString("0.00"));
		desc = desc.Replace("[/1]", (magicHeal * GetPowerMult()).ToString("0.00"));
		return desc;
	}
}
