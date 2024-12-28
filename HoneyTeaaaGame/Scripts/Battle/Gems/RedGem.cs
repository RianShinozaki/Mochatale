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
		GemPowerAnimation(new Color(1, 0.1f, 0.1f), damage, Battle.Instance.GetEnemy(-1).GlobalPosition, cost);
		await ToSignal(this, "AnimationFinished");

		//Inflict damage on enemy
		Enemy theEnemy = Battle.Instance.GetEnemy(-1);

		bool wait = false;
		if(GameController.Instance.godMode) {
			wait = theEnemy.TakeDamage(9999);
		}
		else
			wait = theEnemy.TakeDamage(damage * GetPowerMult());
		
		GetParent<Node>().RemoveChild(this);
		if(wait)
			await ToSignal(theEnemy, "AnimationFinished");
		EmitSignal(SignalName.FinishedTrigger);
		QueueFree();

    }
	public override string GetDescription() {
		string desc = description;
		desc = desc.Replace("[/0]", GetPowerMult().ToString("0.00"));
		desc = desc.Replace("[/1]", (damage * GetPowerMult()).ToString("0.00"));
		desc = desc.Replace("[/2]", (cost * GetPowerMult()).ToString("0.00"));
		return desc;
	}
}
