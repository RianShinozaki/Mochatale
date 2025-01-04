using Godot;
using System;

public partial class Enemy : Node2D
{
	public enum StatusEffect {
		None,
		Burn,
		Drench,
		Shock,
		Bind,
		Instakill
	}

	// SIGNALS
	[Signal]
	public delegate void EnemyTurnFinishedEventHandler();
	[Signal]
	public delegate void AnimationFinishedEventHandler();

	// EXPORT VARIABLES
	[ExportGroup("Constant Variables")]
	[Export] public string MyDialogueDataResource;
	[Export] public string name;
	[Export] public float maxHP;
	[Export] public int level;
	[Export(PropertyHint.MultilineText)] public string description;
	[Export] public int expWorth;

	[ExportGroup("Dynamic Variables")]
	[Export] public float effectLevel = 0;

	[Export] public StatusEffect currentEffect;


	// EXPORT REFERENCES
	[ExportGroup("Constant References")]

	[Export] public Texture2D battleImage;
	[Export] AudioStream hurtSound;
	[Export] AudioStream dieSound;

	// MEMBER VARIABLES
	float HP;
	public bool active = true;
	float shakeLevel;
	public float cumulativeDamage;

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

		if(currentEffect != StatusEffect.None) {
			GetNode<Sprite2D>("EnemyData/ProgressBar/FXIcon").Visible = true;
			GetNode<Label>("EnemyData/ProgressBar/FXIcon/FXLvl").Text = effectLevel.ToString("0.00");
		} else {
			GetNode<Sprite2D>("EnemyData/ProgressBar/FXIcon").Visible = false;
		}
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
		shakeLevel = Mathf.MoveToward(shakeLevel, 0, (float)delta*20);
		sprite.Offset = new Vector2(rand.RandfRange(-shakeLevel, shakeLevel), 0);
    }
    public bool TakeDamage(float damage, StatusEffect type = StatusEffect.None) {
		
		if(type == StatusEffect.Shock && currentEffect == StatusEffect.Drench) {
			damage *= 1 + effectLevel*0.5f;
			effectLevel = 0;
			currentEffect = StatusEffect.None;
		}
		else if(type == StatusEffect.None && currentEffect == StatusEffect.Shock) {
			damage *= 1 + effectLevel*0.5f;
			effectLevel = 0;
			currentEffect = StatusEffect.None;
		}
		else if(type == StatusEffect.Burn && currentEffect == StatusEffect.Bind) {
			damage *= 1 + effectLevel*0.5f;
			effectLevel = 0;
			currentEffect = StatusEffect.None;
		}
		else if(type == StatusEffect.Burn && currentEffect == StatusEffect.Drench) {
			damage = 0;
			effectLevel = 0;
			currentEffect = StatusEffect.None;
		}
		else if(type == StatusEffect.Drench && currentEffect == StatusEffect.Bind) {
			damage *= 1 + effectLevel*0.5f;
			effectLevel = 0;
			currentEffect = StatusEffect.None;
		}
		else if(type == StatusEffect.Bind && currentEffect == StatusEffect.Drench) {
			Battle.Instance.player.ChangeHP(damage/2);
			effectLevel = 0;
			currentEffect = StatusEffect.None;
		}
		else if(type == StatusEffect.Instakill) {
			if(cumulativeDamage >= maxHP/2)
				damage = 9999;
			else
				damage = 0;
		}
		else if(type == currentEffect) {
			effectLevel += damage/4;
		}
		else if(type != Enemy.StatusEffect.None) {
			effectLevel = damage/4;
			currentEffect = type;
		}

		anim.Play("default");
		anim.Play("Hit");
		HP = Mathf.Clamp(HP - damage, 0, maxHP);
		cumulativeDamage += damage;
		progBar.Value = HP/maxHP;
		progBar.GetNode<Label>("Mult").Text = "Health: " + HP + " / " + maxHP;
		shakeLevel = 10;

		if(currentEffect != StatusEffect.None) {
			GetNode<Sprite2D>("EnemyData/ProgressBar/FXIcon").Visible = true;
			GetNode<Sprite2D>("EnemyData/ProgressBar/FXIcon").Frame = (int)currentEffect-1;
			GetNode<Label>("EnemyData/ProgressBar/FXIcon/FXLvl").Text = effectLevel.ToString("0.00");
		} else {
			GetNode<Sprite2D>("EnemyData/ProgressBar/FXIcon").Visible = false;
		}


		if(HP <= 0) {
			Die();
			return true;
		}
		else {
			SFXController.PlaySound(hurtSound);
			EmitSignal(SignalName.AnimationFinished);
			return false;
		}
	}
	public virtual string GetDescription() { return description;}

	public async virtual void EnemyTurn() {
	}
	public async void Die() {
		SFXController.PlaySound(dieSound);
		active = false;
		Battle.Instance.expEarned += expWorth;
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
