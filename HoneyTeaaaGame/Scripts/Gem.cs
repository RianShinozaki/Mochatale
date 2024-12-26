using Godot;
using System;

public partial class Gem : TextureRect
{
	[Export(PropertyHint.MultilineText)] public string description;
	public AnimationPlayer anim;

	[Signal]
    public delegate void FinishedTriggerEventHandler();
	[Signal]
	public delegate void AnimationFinishedEventHandler();
	public float scaleup = 0.5f;
	public override void _Ready()
	{
		MouseEntered += _on_mouse_entered;
		MouseExited += _on_mouse_exited;
		GuiInput += _on_gui_input;
		anim = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Process(double delta)
	{
	}
	public virtual void _on_gui_input(InputEvent @event) {
		if(Battle.Instance.currentPhase != Battle.Phase.PlayerChoice) return;

		if(@event is InputEventMouseButton mb) {
			if(mb.Pressed && mb.ButtonIndex == MouseButton.Left) {
				if(GetParent().Name == "Hand") {
					Battle.Instance.PrimeGem(this);
					anim.Play("Gems/Select");
				} else {
					Battle.Instance.DeprimeGem(this);
					anim.Play("Gems/Deselect");

				}
			}
		}
	}
	public async virtual void Trigger() {}
	public virtual void _on_mouse_entered() {
		Battle.Instance.SetDescription(this, GetDescription());
		Position += Godot.Vector2.Up*10;
	}
	public virtual void _on_mouse_exited() {
		Position -= Godot.Vector2.Up*10;
		if(Battle.Instance.focusedObject == this) {
			Battle.Instance.SetDescription(null);
		}
	}
	public virtual string GetDescription() {return "";}
	public float GetMult() {
		if(GetParent().Name == "Hand") {
			return 1 + scaleup * Battle.Instance.activeHand.GetChildCount();
		} else {
			return scaleup * GetIndex() + Battle.Instance.multAmount;
		}
	}
	public Node2D CreatePowerParticles(Color color, Vector2 position, Node parent, string text, float scale = 0.5f, int zIndex = 0) {
		Node2D parts = Battle.Instance.powerFX.Instantiate() as Node2D;
		parts.Modulate = color;
		parts.Scale = Vector2.One * scale;
		parts.GetNode<Label>("Number").Text = text;
		parent.AddChild(parts);
		parts.GlobalPosition = position;
		parts.ZIndex = zIndex;
		return parts;
	}

	public async void GemPowerAnimation(Color parts1Color, float parts1Value, Vector2 parts1dest, bool useMagic = true) {
		
		anim.Play("Gems/Trigger");
        await ToSignal(anim, "animation_finished");
		Visible = false;

		//Create parts1
		Vector2 pos = GlobalPosition + new Vector2(40, 40);
		Node2D powerFX = CreatePowerParticles(parts1Color, pos, (Node2D)Battle.Instance, parts1Value.ToString());

		//Move parts1 next to white particles
		var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(powerFX, "global_position", Battle.Instance.mult.GlobalPosition + new Vector2(300, 20), 0.25f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");

		/*//Create the white mult particles
		pos = Battle.Instance.mult.GlobalPosition + new Vector2(165, 20);
		Node2D multFX = CreatePowerParticles(Colors.White, pos, Battle.Instance.playerUI, "", 0.25f, -1);

		//Move the white mult particles to parts1
		tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(multFX, "global_position", powerFX.GlobalPosition, 0.25f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");
		GD.Print("White particles moved??");
		multFX.QueueFree();*/

		await ToSignal(GetTree().CreateTimer(0.25f * Battle.Instance.speedMult), "timeout");

		powerFX.GetNode<Label>("Number").Text = (parts1Value * GetMult()).ToString();

		//Make parts1 bigger
		tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Elastic);
		tween.TweenProperty(powerFX, "scale", Vector2.One * (0.5f + (GetMult()-1)*0.08f), 0.25f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");

		if(useMagic) {
			float trueCost = parts1Value * Battle.Instance.costMult;

			if(Battle.Instance.player.MP < trueCost) {
				Battle.Instance.attacking = false;
				powerFX.QueueFree();
				QueueFree();
				EmitSignal(SignalName.FinishedTrigger);
				return;
			}

			Battle.Instance.player.ChangeMagic(-trueCost);

			//Create the cyan magic particles
			pos = Battle.Instance.player.magicBar.GlobalPosition;
			Node2D magicFX = CreatePowerParticles(Colors.Cyan, pos, (Node2D)Battle.Instance, trueCost.ToString(), 0.5f, 5);

			//Move the cyan magic particles to parts1
			tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Quint);
			tween.TweenProperty(magicFX, "global_position", powerFX.GlobalPosition, 0.5f * Battle.Instance.speedMult);
			await ToSignal(tween, "finished");
			magicFX.QueueFree();
		}

		//Move the red power particles to loc
		tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Quint);
		tween.TweenProperty(powerFX, "global_position", parts1dest, 0.5f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");
		powerFX.QueueFree();

		EmitSignal(SignalName.AnimationFinished);
		
	}
}
