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

		gemPool = new Godot.Collections.Array<String>();
		removedGems = new Godot.Collections.Array<String>();
		pocketedGems = new Godot.Collections.Array<Gem>();
		foreach(KeyValuePair<String, PackedScene> gem in gems) {
			for(int i = 0; i < 4; i++) {
				gemPool.Add(gem.Key);
			}
		}
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
			hitFXAnim.Play("default");
			hitFXAnim.Play("Hit");
			
			await ToSignal(hitFXAnim, "animation_finished");
			EmitSignal(SignalName.AnimationEnded);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
