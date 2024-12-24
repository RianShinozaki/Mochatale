using Godot;
using System;

public partial class WhiteGem : Gem
{
	[Export] public float addMult = 0.5f;
	[Export] public float cost = 2;
    public async override void Trigger()
    {
		anim.Play("Gems/Trigger");
        await ToSignal(anim, "animation_finished");
		Visible = false;

		//Create the white addMult particles
		Vector2 pos = GlobalPosition + new Vector2(40, 40);
		Node2D powerFX = CreatePowerParticles(new Color(1, 0.9f, 0.9f), pos, (Node2D)Battle.Instance, (addMult).ToString());

		//Move white addMult particles next to white particles
		var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(powerFX, "global_position", Battle.Instance.mult.GlobalPosition + new Vector2(300, 20), 0.25f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");

		//Create the white mult particles
		pos = Battle.Instance.mult.GlobalPosition + new Vector2(165, 20);
		Node2D multFX = CreatePowerParticles(Colors.White, pos, (Node2D)Battle.Instance, "", 0.25f, -1);

		//Move the white mult particles to red power
		tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(multFX, "global_position", powerFX.GlobalPosition, 0.25f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");
		multFX.QueueFree();

		powerFX.GetNode<Label>("Number").Text = (addMult * GetMult()).ToString();

		//Make the red power bigger
		tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Elastic);
		tween.TweenProperty(powerFX, "scale", Vector2.One * (0.5f + (GetMult()-1)*0.05f), 0.25f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");

		float trueCost = cost * Battle.Instance.costMult;

		if(Battle.Instance.player.MP < trueCost) {
			Battle.Instance.attacking = false;
			powerFX.QueueFree();
			GD.Print("No magic");
			QueueFree();
			EmitSignal(SignalName.FinishedTrigger);
			return;
		}

		Battle.Instance.player.ChangeMagic(-trueCost);

		//Create the cyan magic particles
		pos = Battle.Instance.player.magicBar.GlobalPosition;
		Node2D magicFX = CreatePowerParticles(Colors.Cyan, pos, (Node2D)Battle.Instance, trueCost.ToString(), 0.5f, 5);

		//Move the cyan magic particles to red power particles
		tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Quint);
		tween.TweenProperty(magicFX, "global_position", powerFX.GlobalPosition, 0.5f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");
		magicFX.QueueFree();

		pos = Battle.Instance.mult.GlobalPosition + new Vector2(165, 20);
		//Move the white addMult particles to mult
		tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Quint);
		tween.TweenProperty(powerFX, "global_position", pos, 0.75f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");
		powerFX.QueueFree();

		//Inflict damage on enemy
		Battle.Instance.multAmount += addMult * GetMult() - Battle.Instance.multIncrement;
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
