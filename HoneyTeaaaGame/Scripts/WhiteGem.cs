using Godot;
using System;

public partial class WhiteGem : Gem
{
	[Export] public float addMult = 0.5f;
	[Export] public float cost = 2;
	AudioStream triggerSound;
    public override void _Ready()
    {
        base._Ready();
		triggerSound = GD.Load<AudioStream>("res://Audio/Sounds/093 Shield Reflect.wav");
    }
    public async override void Trigger()
    {
		//Play the animation
		GemPowerAnimation(new Color(1, 0.9f, 0.9f), addMult, Battle.Instance.mult.GlobalPosition + new Vector2(165, 20));
		await ToSignal(this, "AnimationFinished");

		//Add multiplier on enemy
		Battle.Instance.multAmount += addMult * GetMult() - Battle.Instance.multIncrement;
		SFXController.PlaySound(triggerSound);

		GetParent<Node>().RemoveChild(this);
		QueueFree();
		EmitSignal(SignalName.FinishedTrigger);
    }
	public override string GetDescription() {
		string desc = description;
		desc = desc.Replace("[/0]", GetMult().ToString("0.00"));
		desc = desc.Replace("[/1]", (addMult * GetMult()).ToString("0.00"));
		desc = desc.Replace("[/2]", (cost * GetMult()).ToString("0.00"));
		return desc;
	}
}
