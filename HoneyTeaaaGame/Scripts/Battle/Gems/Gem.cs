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
	public AudioStream selectSound;
	public float scaleup = 0.5f;
	public float costMult = 1;
	public float powerMult = 1;
	public override void _Ready()
	{
		MouseEntered += _on_mouse_entered;
		MouseExited += _on_mouse_exited;
		GuiInput += _on_gui_input;
		anim = GetNode<AnimationPlayer>("AnimationPlayer");
		selectSound = GD.Load<AudioStream>("res://Audio/Sounds/Select.wav");
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
					SFXController.PlaySound(selectSound);
				} else {
					Battle.Instance.DeprimeGem(this);
					anim.Play("Gems/Deselect");

				}
			}
		}
	}
	public async virtual void Trigger() {}
	public async virtual void Fail() {
		anim.Play("Gems/Trigger");
        await ToSignal(anim, "animation_finished");
		Visible = false;
		
		//Inflict damage on enemy
		GetParent<Node>().RemoveChild(this);
		QueueFree();
		EmitSignal(SignalName.FinishedTrigger);
	}
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
	public float GetPowerMult() {
		float mult = 0f;
		if(GetParent().Name == "Hand") {
			mult = 1 + scaleup * Battle.Instance.activeHand.GetChildCount();
		} else {
			mult = scaleup * GetIndex() + Battle.Instance.seqPowerMult;
		}
		return mult * powerMult;
	}
	public float GetCostMult() {
		float mult = 0f;
		if(GetParent().Name == "Hand") {
			mult = 1 + scaleup * Battle.Instance.activeHand.GetChildCount();
		} else {
			mult = scaleup * GetIndex() + Battle.Instance.seqCostMult;
		}
		return mult;
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
	public void SetMult(float ncostMult, float npowerMult) {
		costMult = ncostMult;
		powerMult = npowerMult;
	}

	public async void GemPowerAnimation(Color parts1Color, float parts1Value, Vector2 parts1dest, float baseCost, bool useMagic = true) {
		
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

		await ToSignal(GetTree().CreateTimer(0.25f * Battle.Instance.speedMult), "timeout");

		powerFX.GetNode<Label>("Number").Text = (parts1Value * GetPowerMult()).ToString();

		//Make parts1 bigger
		tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Elastic);
		tween.TweenProperty(powerFX, "scale", Vector2.One * (0.5f + (GetPowerMult()-1)*0.08f), 0.25f * Battle.Instance.speedMult);
		await ToSignal(tween, "finished");

		if(useMagic) {
			float trueCost = baseCost * GetCostMult() * costMult;
			GD.Print("baseCost", GetCostMult(), costMult);

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
