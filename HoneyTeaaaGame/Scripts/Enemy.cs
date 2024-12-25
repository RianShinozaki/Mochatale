using Godot;
using System;

public partial class Enemy : Node2D
{
	[Signal]
	public delegate void EnemyTurnFinishedEventHandler();

	[Signal]
	public delegate void AnimationFinishedEventHandler();
	[Export] public string MyDialogueDataResource;
	float HP;
	[Export] public float maxHP;
	[Export] public string name;
	[Export] public int level;
	[Export(PropertyHint.MultilineText)] public string description;

	ProgressBar progBar;
	public AnimationPlayer anim;
	public Label Damage;
	public bool active = true;
    public override void _Ready()
    {
        anim = GetNode<Sprite2D>("HitFX").GetNode<AnimationPlayer>("AnimationPlayer");
		progBar = GetNode<ProgressBar>("ProgressBar");
		HP = maxHP;
		progBar.Value = HP/maxHP;
		progBar.GetNode<Label>("Mult").Text = "Health: " + HP + " / " + maxHP;
		Damage = GetNode<Label>("Label");

		Panel clickBox = GetNode<Panel>("ClickBox");
		clickBox.GuiInput += _on_panel_gui_input;
		clickBox.MouseEntered += _on_mouse_entered;
		clickBox.MouseExited += _on_mouse_exited;

    }
    public void TakeDamage(float damage) {
		anim.Play("default");
		anim.Play("Hit");
		HP = Mathf.Clamp(HP - damage, 0, maxHP);
		progBar.Value = HP/maxHP;
		progBar.GetNode<Label>("Mult").Text = "Health: " + HP + " / " + maxHP;

		if(HP <= 0) Die();
	}
	public virtual string GetDescription() { return description;}

	public async virtual void EnemyTurn() {
		
	}
	public async void Die() {
		active = false;
		GetNode<Sprite2D>("Sprite").Material.Set("blend_mode", 1);
		progBar.Visible = false;
		GetParent().RemoveChild(this);
		Battle.Instance.AddChild(this);
		
		var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(GetNode<Sprite2D>("Sprite"), "modulate", new Color(1, 1, 1, 0), 1f);
		await ToSignal(tween, "finished");
		QueueFree();
		EmitSignal(SignalName.EnemyTurnFinished);
	}
	private void _on_panel_gui_input(InputEvent @event) {
		if(Battle.Instance.currentPhase != Battle.Phase.PlayerChoice) return;

		if(@event is InputEventMouseButton mb) {
			if(mb.Pressed && mb.ButtonIndex == MouseButton.Left) {
				Battle.Instance.SetEnemySelection(GetIndex());
			}
		}
	}
	public virtual void _on_mouse_entered() {
		Battle.Instance.SetDescription(this, GetDescription());
	}
	public virtual void _on_mouse_exited() {
		if(Battle.Instance.focusedObject == this) {
			Battle.Instance.SetDescription(null);
		}
	}
}
