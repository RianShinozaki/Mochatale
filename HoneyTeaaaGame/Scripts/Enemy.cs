using Godot;
using System;

public partial class Enemy : Node2D
{
	public enum StatusEffect {
		None,
		Burn,
		Shock,
		Drench,
		Bind
	}

	// SIGNALS
	[Signal]
	public delegate void EnemyTurnFinishedEventHandler();
	[Signal]
	public delegate void AnimationFinishedEventHandler();

	// EXPORT VARIABLES
	[Export] public string MyDialogueDataResource;
	[Export] public string name;
	[Export] public float maxHP;
	[Export] public int level;
	[Export(PropertyHint.MultilineText)] public string description;
	[Export] public float effectLevel = 0;
	[Export] public StatusEffect currentEffect;

	// EXPORT REFERENCES
	[Export] public Texture2D battleImage;
	[Export] AudioStream hurtSound;
	[Export] AudioStream dieSound;

	// MEMBER VARIABLES
	float HP;
	public bool active = true;
	float shakeLevel;

	// MEMBER REFERENCES
	ProgressBar progBar;
	public AnimationPlayer anim;
	public Label Damage;
	public Sprite2D sprite;
	RandomNumberGenerator rand;
	public AudioStreamPlayer audio;

    public override void _Ready()
    {
        anim = GetNode<AnimationPlayer>("EnemyData/HitFX/AnimationPlayer");
		progBar = GetNode<ProgressBar>("EnemyData/ProgressBar");
		HP = maxHP;
		progBar.Value = HP/maxHP;
		progBar.GetNode<Label>("Mult").Text = "Health: " + HP + " / " + maxHP;

		Panel clickBox = GetNode<Panel>("EnemyData/ClickBox");
		clickBox.GuiInput += _on_panel_gui_input;
		clickBox.MouseEntered += _on_mouse_entered;
		clickBox.MouseExited += _on_mouse_exited;

		sprite = GetNode<Sprite2D>("EnemyData/Sprite");
		sprite.Texture = battleImage;
		rand = new RandomNumberGenerator();
		audio = GetNode<AudioStreamPlayer>("EnemyData/AudioStreamPlayer");
		dieSound = GD.Load<AudioStream>("res://Audio/Sounds/033 Enemy die.wav");
		
		GetNode<Label>("EnemyData/Name").Text = "Lv. " + level + " " + name;
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
		shakeLevel = Mathf.MoveToward(shakeLevel, 0, (float)delta*20);
		sprite.Offset = new Vector2(rand.RandfRange(-shakeLevel, shakeLevel), 0);
    }
    public void TakeDamage(float damage) {
		anim.Play("default");
		anim.Play("Hit");
		HP = Mathf.Clamp(HP - damage, 0, maxHP);
		progBar.Value = HP/maxHP;
		progBar.GetNode<Label>("Mult").Text = "Health: " + HP + " / " + maxHP;
		shakeLevel = 10;
		if(HP <= 0) Die();
		else {
			SFXController.PlaySound(hurtSound);
			EmitSignal(SignalName.AnimationFinished);
		}
	}
	public virtual string GetDescription() { return description;}

	public async virtual void EnemyTurn() {
		
	}
	public async void Die() {
		SFXController.PlaySound(dieSound);
		active = false;
		GetNode<Sprite2D>("EnemyData/Sprite").Material.Set("blend_mode", 1);
		progBar.Visible = false;
		GetParent().RemoveChild(this);
		Battle.Instance.AddChild(this);
		Battle.Instance.SelectedEnemyIndex -= 1;
		
		var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(GetNode<Sprite2D>("EnemyData/Sprite"), "modulate", new Color(1, 1, 1, 0), 1f);
		await ToSignal(tween, "finished");
		EmitSignal(SignalName.AnimationFinished);
		QueueFree();
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
