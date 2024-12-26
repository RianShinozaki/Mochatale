using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node2D
{
	[Signal]
	public delegate void AnimationEndedEventHandler();
	public float HP;
	[Export] public float maxHP;
	public float MP;
	[Export] public float maxMP;
	public ProgressBar magicBar;
	public ProgressBar healthBar;
	public AnimationPlayer anim;
	[Export] public Godot.Collections.Dictionary<String, PackedScene> gems;

	public int pullNumber = 6;
	[Export] public Godot.Collections.Array<String> gemPool;
	public Godot.Collections.Array<String> removedGems;
	public Godot.Collections.Array<String> removedGemsTemp;
	public Godot.Collections.Array<Gem> pocketedGems;
	public AnimationPlayer hitFXAnim;
	public Sprite2D sprite;
	float shakeLevel;
	RandomNumberGenerator rand;
	public bool alive = true;

	// SOUNDS

	AudioStream hurtSound;
	AudioStream dieSound;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//anim = GetNode<Sprite2D>("HitFX").GetNode<AnimationPlayer>("AnimationPlayer");
		magicBar = GetNode<ProgressBar>("Description/MagicBar");
		healthBar = GetNode<ProgressBar>("Description/HPBar");
		MP = maxMP;
		HP = maxHP;
		magicBar.GetNode<Label>("Mult").Text = "Magic: " + MP.ToString() + " / " + maxMP.ToString();
		healthBar.GetNode<Label>("Mult").Text = "Health: " + HP.ToString() + " / " + maxHP.ToString();

		hitFXAnim = GetNode<Sprite2D>("HitFX").GetNode<AnimationPlayer>("AnimationPlayer");
		sprite = GetNode<Sprite2D>("MochaBack");
		rand = new RandomNumberGenerator();

		gemPool = new Godot.Collections.Array<String>();
		removedGems = new Godot.Collections.Array<String>();
		pocketedGems = new Godot.Collections.Array<Gem>();
		foreach(KeyValuePair<String, PackedScene> gem in gems) {
			for(int i = 0; i < 4; i++) {
				gemPool.Add(gem.Key);
			}
		}

		hurtSound = GD.Load<AudioStream>("res://Audio/Sounds/Hurt.wav");
		dieSound = GD.Load<AudioStream>("res://Audio/Sounds/032 Player die.wav");
	}

	public void ChangeMagic(float amount) {
		MP = Mathf.Clamp(MP + amount, 0, maxMP);
		magicBar.Value = MP/maxMP;
		magicBar.GetNode<Label>("Mult").Text = "Magic: " + MP.ToString() + " / " + maxMP.ToString();
	}
	public async void ChangeHP(float amount) {
		HP = Mathf.Clamp(HP + amount, 0, maxHP);
		healthBar.Value = HP/maxHP;
		healthBar.GetNode<Label>("Mult").Text = "Health: " + HP.ToString() + " / " + maxHP.ToString();
		if(amount < 0) {
			shakeLevel = 10;

			hitFXAnim.Play("default");
			hitFXAnim.Play("Hit");
			
			if(HP <= 0) {
				alive = false;
				sprite.GetNode<AnimationPlayer>("AnimationPlayer").Play("Die");
				SFXController.PlaySound(dieSound);
			} else {
				SFXController.PlaySound(hurtSound);

			}

			await ToSignal(hitFXAnim, "animation_finished");
			EmitSignal(SignalName.AnimationEnded);
			
		}
	}

	public void GetCursed(Curse curse) {
		EmitSignal(SignalName.AnimationEnded);
	}

	public override void _Process(double delta)
	{
		shakeLevel = Mathf.MoveToward(shakeLevel, 0, (float)delta*20);
		sprite.Offset = new Vector2(rand.RandfRange(-shakeLevel, shakeLevel), 0);
	}
	public void DrawHand() {
		//Clear any gems from before
		for(int i = 0; i < Battle.Instance.hand.GetChildCount(); i++) {
			Battle.Instance.hand.GetChild(i).QueueFree();
		}
		for(int i = 0; i < Battle.Instance.activeHand.GetChildCount(); i++) {
			Battle.Instance.activeHand.GetChild(i).QueueFree();
		}

		removedGemsTemp = removedGems;
		removedGems.Clear();
		for(int i = 0; i < pullNumber - pocketedGems.Count; i++) {
			RandomNumberGenerator rand = new RandomNumberGenerator();
			int pull = rand.RandiRange(0, gemPool.Count-1);

			Gem thisGem = gems[gemPool[pull]].Instantiate() as Gem;
			Battle.Instance.hand.AddChild(thisGem);
			removedGems.Add(gemPool[pull]);
			gemPool.RemoveAt(pull);
		}
		for(int i = 0; i < pocketedGems.Count; i++) {
			Battle.Instance.hand.AddChild(pocketedGems[i]);
		}
		pocketedGems.Clear();
		for(int i = 0; i < removedGemsTemp.Count; i++) {
			gemPool.Add(removedGemsTemp[i]);
		}

	}
}
