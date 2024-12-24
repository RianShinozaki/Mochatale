using Godot;
using System;

public partial class BlueGem : Gem
{
	[Export] public float magicHeal = 4;
	
    public async override void Trigger()
    {
		anim.Play("Gems/Trigger");
        await ToSignal(anim, "animation_finished");
		Visible = false;

		//Create the cyan magic particles
		Vector2 pos = GlobalPosition + new Vector2(40, 40);
		Node2D powerFX = CreatePowerParticles(Colors.Cyan, pos, (Node2D)Battle.Instance, (magicHeal).ToString());

		//Move cyan particles next to white particles
		var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(powerFX, "global_position", Battle.Instance.mult.GlobalPosition + new Vector2(300, 20), 0.25f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");

		//Create the white mult particles
		pos = Battle.Instance.mult.GlobalPosition + new Vector2(165, 20);
		Node2D multFX = CreatePowerParticles(Colors.White, pos, (Node2D)Battle.Instance, "", 0.25f, -1);

		//Move the white mult particles to cyan magic
		tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(multFX, "global_position", powerFX.GlobalPosition, 0.25f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");
		multFX.QueueFree();

		powerFX.GetNode<Label>("Number").Text = (magicHeal * GetMult()).ToString();

		//Make the cyan power bigger
		tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Elastic);
		tween.TweenProperty(powerFX, "scale", Vector2.One * (0.5f + (GetMult()-1)*0.08f), 0.25f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");

		//Move the cyan magic particles to enemy
		tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Quint);
		tween.TweenProperty(powerFX, "global_position", Battle.Instance.player.magicBar.GlobalPosition, 0.5f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");
		powerFX.QueueFree();

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
