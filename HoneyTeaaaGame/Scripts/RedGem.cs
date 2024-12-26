using Godot;
using System;

public partial class RedGem : Gem
{
	[Export] public float damage = 4;
	[Export] public float cost = 4;
	
    public async override void Trigger()
    {
		//Select target
		if(Battle.Instance.enemies.GetChildCount() == 0) {
			GetParent<Node>().RemoveChild(this);
			QueueFree();
			EmitSignal(SignalName.FinishedTrigger);
		}
		if(Battle.Instance.SelectedEnemyIndex > Battle.Instance.enemies.GetChildCount()) {
			Battle.Instance.SelectedEnemyIndex = 0;
		}

		//Play the animation
		GemPowerAnimation(new Color(1, 0.1f, 0.1f), damage, Battle.Instance.GetEnemy(-1).GlobalPosition);
		await ToSignal(this, "AnimationFinished");

		//Inflict damage on enemy
		Battle.Instance.GetEnemy(-1).TakeDamage(damage * GetMult());
		GetParent<Node>().RemoveChild(this);
		QueueFree();
		EmitSignal(SignalName.FinishedTrigger);

    }
	public override string GetDescription() {
		string desc = description;
		desc = desc.Replace("[/0]", GetMult().ToString("0.00"));
		desc = desc.Replace("[/1]", (damage * GetMult()).ToString("0.00"));
		desc = desc.Replace("[/2]", (cost * GetMult()).ToString("0.00"));
		return desc;
	}
}
