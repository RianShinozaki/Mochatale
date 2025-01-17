using Godot;
using System;
using System.Collections.Generic;
using System.Security.Principal;

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
	[Export] public Godot.Collections.Array<PackedScene> gems;
	[Export] public Godot.Collections.Array<PackedScene> gemsEquipped;
	public int pullNumber = 6;
	[Export] public Godot.Collections.Array<int> gemPool;
	public Godot.Collections.Array<int> removedGems;
	public Godot.Collections.Array<int> removedGemsTemp;
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

		PlayerLevelData currentLevel = PlayerData.GetLevelData();
		maxMP = currentLevel.maxMP;
		maxHP = currentLevel.maxHP;
		pullNumber = currentLevel.handSize;
		MP = maxMP;
		HP = maxHP;
		if(GameController.Instance.godMode) {
			pullNumber = 12;
		}
		magicBar.GetNode<Label>("Mult").Text = "Magic: " + MP.ToString() + " / " + maxMP.ToString();
		healthBar.GetNode<Label>("Mult").Text = "Health: " + HP.ToString() + " / " + maxHP.ToString();

		hitFXAnim = GetNode<Sprite2D>("HitFX").GetNode<AnimationPlayer>("AnimationPlayer");
		sprite = GetNode<Sprite2D>("MochaBack");
		rand = new RandomNumberGenerator();

		gemPool = new Godot.Collections.Array<int>();
		removedGems = new Godot.Collections.Array<int>();
		pocketedGems = new Godot.Collections.Array<Gem>();

		gemsEquipped = PlayerData.Instance.gemsEquipped;
		for(int ii = 0; ii < gemsEquipped.Count; ii++) {
			for(int i = 0; i < 2; i++) {
				gemPool.Add(ii);
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

		//removedGemsTemp = removedGems;
		//removedGems.Clear();
		for(int i = 0; i < pullNumber - pocketedGems.Count; i++) {
			RandomNumberGenerator rand = new RandomNumberGenerator();
			int pull = rand.RandiRange(0, gemPool.Count-1);
			Gem thisGem = gemsEquipped[gemPool[pull]].Instantiate() as Gem;
			Battle.Instance.hand.AddChild(thisGem);
			removedGems.Add(gemPool[pull]);
			int thePull = gemPool[pull];
			gemPool.RemoveAt(pull);
			GD.Print(thePull, " ", gemPool);

		}
		for(int i = 0; i < pocketedGems.Count; i++) {
			Battle.Instance.hand.AddChild(pocketedGems[i]);
		}
		pocketedGems.Clear();
		for(int i = 0; i < removedGems.Count; i++) {
			gemPool.Add(removedGems[i]);
		}
		removedGems.Clear();

	}
}
