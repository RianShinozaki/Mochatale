using Godot;
using System;

public partial class Gem : TextureRect
{
	[Export(PropertyHint.MultilineText)] public string description;
	public AnimationPlayer anim;

	[Signal]
    public delegate void FinishedTriggerEventHandler();
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
		Battle.Instance.SetFocusedGem(this);
		Position += Godot.Vector2.Up*10;
	}
	public virtual void _on_mouse_exited() {
		Position -= Godot.Vector2.Up*10;
		if(Battle.Instance.focusedGem == this) {
			Battle.Instance.SetFocusedGem(null);
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
	public Node2D CreatePowerParticles(Color color, Vector2 position, Node2D parent, string text, float scale = 0.5f, int zIndex = 0) {
		Node2D parts = Battle.Instance.powerFX.Instantiate() as Node2D;
		parts.Modulate = color;
		parts.Scale = Vector2.One * scale;
		parts.GetNode<Label>("Number").Text = text;
		parent.AddChild(parts);
		parts.GlobalPosition = position;
		parts.ZIndex = zIndex;
		return parts;
	}
}
