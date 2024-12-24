using Godot;
using System;

public partial class Battle : Node2D
{
	public enum Phase {
		ToPlayerChoice,
		PlayerChoice,
		PocketingGems,
		PlayerAttacking,
		ToEnemyChoice,
		EnemyChoice,
		EnemyAttacking
	}
	public Phase currentPhase = Phase.PlayerChoice;
	public static Battle Instance;
	[Export] public HBoxContainer hand;
	[Export] public HBoxContainer activeHand;
	[Export] public RichTextLabel itemDescription;
	[Export] public Enemy enemy;
	[Export] public Player player;
	[Export] public Label mult;
	[Export] public Control battleOptions;
	[Export] public Control playerUI;
	public float multAmount = 1;
	public float costMult = 1;
	public float multIncrement = 0.5f;
	public float speedMult = 1f;
	[Export] public PackedScene powerFX; 

	public bool attacking = false;
	public Gem focusedGem;

    public override void _Ready()
    {
		Instance = this;
        base._Ready();
		mult = GetNode<Label>("Mult");
		mult.Visible = false;
		SwitchState(Phase.ToPlayerChoice);
    }
    public override void _Process(double delta)
    {
        
    }
    public void SetFocusedGem(Gem gem) {
		focusedGem = gem;
		if(gem != null) {
			itemDescription.Text = gem.GetDescription();
			itemDescription.GetParent<Control>().Visible = true;
		} else {
			itemDescription.Text = null;
			itemDescription.GetParent<Control>().Visible = false;

		}
	}
	public void _on_attack_pressed() {
		DoAttack();
	}
	public void _on_pocket_pressed() {
		battleOptions.GetNode<Button>("Attack").Text = "SKIP";
		while(activeHand.GetChildCount() > 0) {
			Gem thisGem = activeHand.GetChild<Gem>(0);
			player.pocketedGems.Add(activeHand.GetChild<Gem>(0));
			activeHand.RemoveChild(activeHand.GetChild<Gem>(0));
			thisGem.GlobalPosition = new Vector2(-100, -100);
		}
	}
	public async void SwitchState(Phase newState) {
		currentPhase = newState;
		Tween tween;
		switch(currentPhase) {
			case Phase.ToPlayerChoice:
				battleOptions.Visible = true;
				playerUI.Position = new Vector2(0, 330);
				battleOptions.GetNode<Button>("Attack").Text = "SKIP";
				battleOptions.GetNode<Button>("Pocket").Disabled = true;

				player.DrawHand();

				tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Elastic);
				tween.TweenProperty(playerUI, "position", new Vector2(0, 0), 1f);
				await ToSignal(tween, "finished");

				//player.ChangeMagic(player.maxMP/4);
				currentPhase = Phase.PlayerChoice;
				break;
			case Phase.PlayerChoice:
				battleOptions.Visible = true;
				break;
			case Phase.PlayerAttacking:
				battleOptions.Visible = false;
				break;
			case Phase.ToEnemyChoice:
				
				playerUI.Position = new Vector2(0, 0);

				tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Quart);
				tween.TweenProperty(playerUI, "position", new Vector2(0, 330), 0.5f);
				await ToSignal(tween, "finished");

				battleOptions.Visible = false;

				SwitchState(Phase.EnemyChoice);
				break;
			case Phase.EnemyChoice:
				enemy.CallDeferred("EnemyTurn");
				await ToSignal(enemy, "EnemyTurnFinished");
				SwitchState(Phase.ToPlayerChoice);

				break;
		}
	}
	public async void DoAttack() {
		SwitchState(Phase.PlayerAttacking);

		mult.Visible = true;
		attacking = true;
		Node2D multFX = Battle.Instance.powerFX.Instantiate() as Node2D;
		multFX.Modulate = new Color(1, 1, 1);
		multFX.GetNode<Label>("Number").Text = "x1.00";
		AddChild(multFX);
		multFX.GlobalPosition = mult.GlobalPosition + new Vector2(155, 20);
		speedMult = 0.8f;

		while(attacking && activeHand.GetChildCount() > 0) {
			multFX.GetNode<Label>("Number").Text = "x" + multAmount.ToString();
			Gem thisGem = (Gem)activeHand.GetChild(0);
			thisGem.CallDeferred("Trigger");
			await ToSignal(thisGem, "FinishedTrigger");

			if(attacking) {
				multAmount += multIncrement;
				costMult += multIncrement;
				var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Elastic);
				tween.TweenProperty(multFX, "scale", Vector2.One * Mathf.Sqrt(multAmount*0.5f) * 0.5f, 0.25f * speedMult);
				await ToSignal(tween, "finished");
				speedMult = Mathf.Clamp(speedMult-0.1f, 0.3f, 1);
			}
		}

		GD.Print("end attack");
		multAmount = 1;
		costMult = 1;
		mult.Visible = false;
		multFX.QueueFree();

		SwitchState(Phase.ToEnemyChoice);
	}
	public void PrimeGem(Gem gem) {
		hand.RemoveChild(gem);
		activeHand.AddChild(gem);
		battleOptions.GetNode<Button>("Attack").Text = "ATTACK";
		if(activeHand.GetChildCount() == 1 || activeHand.GetChildCount() == 2) {
			battleOptions.GetNode<Button>("Pocket").Disabled = false;
		} else {
			battleOptions.GetNode<Button>("Pocket").Disabled = true;
		}
	}
	public void DeprimeGem(Gem gem) {
		activeHand.RemoveChild(gem);
		hand.AddChild(gem);
		if(activeHand.GetChildCount() == 0) {
			battleOptions.GetNode<Button>("Attack").Text = "SKIP";
		}
		if(activeHand.GetChildCount() == 1 || activeHand.GetChildCount() == 2) {
			battleOptions.GetNode<Button>("Pocket").Disabled = false;
		} else {
			battleOptions.GetNode<Button>("Pocket").Disabled = true;
		}
	}

	
}
