using Godot;
using System;
using System.Diagnostics;

public partial class Battle : Node2D
{
	public enum Phase {
		BattleIntro,
		ToPlayerChoice,
		PlayerChoice,
		PocketingGems,
		PlayerAttacking,
		EnemyChoice,
		EnemyAttacking,
		PlayerWins,
		PlayerLose
	}
	public Phase currentPhase = Phase.PlayerChoice;
	public static Battle Instance;
	[Export] public HBoxContainer hand;
	[Export] public HBoxContainer activeHand;
	[Export] public RichTextLabel itemDescription;
	[Export] public Player player;
	[Export] public Label mult;
	[Export] public Control battleOptions;
	[Export] public Control playerUI;
	public float seqPowerMult = 1;
	public float seqCostMult = 1;
	public float multIncrement = 0.5f;
	public float speedMult = 1f;
	[Export] public PackedScene powerFX; 
	[Export] string introDialogueID;
	public Node2D enemies;
	[Export] public int SelectedEnemyIndex = 0;

	public bool attacking = false;
	public Node focusedObject;
	public Sprite2D reticle;

	[Export] public Godot.Collections.Array<Curse> curses;
	Godot.Collections.Array<PackedScene> theseEnemies;
	Node2D GameOver;

	// EXPORT VARIABLES
	[Export] public Godot.Collections.Array<Vector2> spawnPos;

	// SOUNDS

	public AudioStream menuSwish;
	public AudioStream startPlayerAttack;
	public AudioStream startBattle;

    public override void _Ready()
    {
		Instance = this;
        base._Ready();
		mult = GetNode<Label>("PlayerUI/Mult");
		mult.Visible = false;
		enemies = GetNode<Node2D>("Enemies");
		reticle = GetNode<Sprite2D>("Reticle");
		
		reticle.Visible = false;

		menuSwish = GD.Load<AudioStream>("res://Audio/Sounds/Swish.wav");
		startPlayerAttack = GD.Load<AudioStream>("res://Audio/Sounds/029 Player try PSI.wav");
		startBattle = GD.Load<AudioStream>("res://Audio/Sounds/snd_weaponpull_ch1.wav");

		curses = new Godot.Collections.Array<Curse>();
		GameOver = GetNode<Node2D>("Game Over");

    }

    public override void _Process(double delta)
    {
        
    }
    public void SetDescription(Node node, String display = "") {
		if(currentPhase != Phase.PlayerChoice) return;
		focusedObject = node;
		if(node != null) {
			itemDescription.Text = display;
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
		battleOptions.GetNode<Button>("Pocket").Disabled = true;
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
		SceneTreeTimer timer;
		switch(currentPhase) {
			case Phase.BattleIntro:
				GameOver.Visible = false;
				SFXController.PlaySound(startBattle);
				Vector2 playerToPos = player.GlobalPosition;
				player.GlobalPosition -= Vector2.Right * 400;
				Vector2 enemyToPos = enemies.GlobalPosition;
				enemies.GlobalPosition += Vector2.Right * 400;
				playerUI.Position = new Vector2(0, 300);
				player.GetNode<Control>("Description").Visible = false;
				player.sprite.Modulate = Colors.White;
				GetNode<ColorRect>("Fade").Color = new Color(0, 0, 0, 1);

				GetParent<CanvasLayer>().Visible = true;
				timer = GetTree().CreateTimer(0.1f);
				await ToSignal(timer, "timeout");
				GetParent<CanvasLayer>().Visible = false;
				timer = GetTree().CreateTimer(0.1f);
				await ToSignal(timer, "timeout");
				GetParent<CanvasLayer>().Visible = true;
				timer = GetTree().CreateTimer(0.1f);
				await ToSignal(timer, "timeout");
				GetParent<CanvasLayer>().Visible = false;
				timer = GetTree().CreateTimer(0.1f);
				await ToSignal(timer, "timeout");
				GetParent<CanvasLayer>().Visible = true;

				tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Cubic);
				tween.TweenProperty(GetNode<ColorRect>("Fade"), "color", new Color(0, 0, 0, 0), 0.5f);
				
				tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Cubic).SetParallel(true);
				tween.TweenProperty(player, "global_position", playerToPos, 1f);
				tween.TweenProperty(enemies, "global_position", enemyToPos, 1f);

				DialogueBridge.Instance.SwapDialogueBoxData(GameController.atkDialogueResPath);
				DialogueBridge.Instance.SetVariable("EnemyName", 0, GetEnemy(0).name);
				DialogueBridge.Instance.StartDialogueID(introDialogueID);
				await ToSignal(DialogueBridge.Instance.dialogueBox, "dialogue_ended");

				player.GetNode<Control>("Description").Visible = true;
				SwitchState(Phase.ToPlayerChoice);
				break;

			case Phase.ToPlayerChoice:
				battleOptions.Visible = false;
				playerUI.Position = new Vector2(0, 300);
				battleOptions.GetNode<Button>("Attack").Text = "SKIP";
				battleOptions.GetNode<Button>("Pocket").Disabled = true;

				RichTextLabel rtl = playerUI.GetNode<RichTextLabel>("StatusFX");
				rtl.Text = "[wave amp=25.0 freq=5.0 connected=1]";
				foreach(Curse curse in curses) {
					rtl.Text += curse.tag + "\n";
				}
				rtl.Text += "[/wave]";

				player.DrawHand();

				SFXController.PlaySound(menuSwish);
				tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Elastic);
				tween.TweenProperty(playerUI, "position", new Vector2(0, 0), 0.75f);
				await ToSignal(tween, "finished");

				if(player.MP < player.maxMP/5)
					player.ChangeMagic(player.maxMP/5 - player.MP);
				SwitchState(Phase.PlayerChoice);
				break;

			case Phase.PlayerChoice:
				reticle.GlobalPosition = GetEnemy(SelectedEnemyIndex).GlobalPosition;
				reticle.Visible = true;
				battleOptions.Visible = true;
				break;

			case Phase.PlayerAttacking:
				SetDescription(null);
				reticle.Visible = false;
				battleOptions.Visible = false;
				break;

			case Phase.EnemyChoice:
				
				playerUI.Position = new Vector2(0, 0);

				SFXController.PlaySound(menuSwish);
				tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Quart);
				tween.TweenProperty(playerUI, "position", new Vector2(0, 300), 0.5f);
				await ToSignal(tween, "finished");

				for(int i = 0; i < enemies.GetChildCount(); i++) {
					if(!player.alive) break;
					Enemy enemy = GetEnemy(i);
					enemy.CallDeferred("EnemyTurn");
					await ToSignal(enemy, "EnemyTurnFinished");
				}

				if(!player.alive) SwitchState(Phase.PlayerLose);
				else if(enemies.GetChildCount() == 0) SwitchState(Phase.PlayerWins);
				else SwitchState(Phase.ToPlayerChoice);

				break;
			case Phase.PlayerWins:
				DialogueBridge.Instance.SwapDialogueBoxData(GameController.atkDialogueResPath);
				player.GetNode<Control>("Description").Visible = false;
				DialogueBridge.Instance.StartDialogueID("WinGeneric");

				await ToSignal(DialogueBridge.Instance.dialogueBox, "dialogue_ended");
				break;

			case Phase.PlayerLose:
				GetNode<ColorRect>("Fade").Color = Colors.Black;
				player.GetNode<Control>("Description").Visible = false;

				timer = GetTree().CreateTimer(1);
				await ToSignal(timer, "timeout");

				DialogueBridge.Instance.SwapDialogueBoxData(GameController.atkDialogueResPath);
				DialogueBridge.Instance.StartDialogueID("Lose");

				await ToSignal(DialogueBridge.Instance.dialogueBox, "dialogue_ended");

				tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Quart);
				tween.TweenProperty(player, "modulate", new Color(1, 1, 1, 0), 1f);
				await ToSignal(tween, "finished");

				GameOver.Visible = true;
				GameOver.Modulate = new Color(1, 1, 1, 0);

				tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Quart);
				tween.TweenProperty(GameOver, "modulate", new Color(1, 1, 1, 1), 1f);
				await ToSignal(tween, "finished");

				break;
		}
	}

	public async void DoAttack() {
		SFXController.PlaySound(startPlayerAttack);
		SwitchState(Phase.PlayerAttacking);

		mult.Visible = true;
		attacking = true;
		Node2D multFX = Battle.Instance.powerFX.Instantiate() as Node2D;
		multFX.Modulate = new Color(1, 1, 1);
		multFX.GetNode<Label>("Number").Text = "x1.00";
		AddChild(multFX);
		multFX.GlobalPosition = mult.GlobalPosition + new Vector2(155, 20);
		speedMult = 0.8f;
		Gem thisGem;

		while(attacking && activeHand.GetChildCount() > 0) {
			bool successful = true;
			thisGem = (Gem)activeHand.GetChild(0);

			foreach(Curse curse in curses) {
				if(curse.CanGemTrigger(thisGem) == false)
					successful = false;
			}
			if(successful) {
				multFX.GetNode<Label>("Number").Text = "x" + seqPowerMult.ToString();

				float curseCostMult = 1;
				float cursePowerMult = 1;
				foreach(Curse curse in curses) {
					curseCostMult *= curse.SetGemCostMult(thisGem);
				}
				foreach(Curse curse in curses) {
					cursePowerMult *= curse.SetGemPowerMult(thisGem);
				}
				thisGem.SetMult(curseCostMult, cursePowerMult);
				thisGem.CallDeferred("Trigger");
				await ToSignal(thisGem, "FinishedTrigger");
				thisGem.SetMult(1, 1);

				if(attacking) {
					seqPowerMult += multIncrement;
					seqCostMult += multIncrement;
					var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Elastic);
					tween.TweenProperty(multFX, "scale", Vector2.One * Mathf.Sqrt(seqPowerMult*0.5f) * 0.5f, 0.25f * speedMult);
					await ToSignal(tween, "finished");
					speedMult = Mathf.Clamp(speedMult-0.1f, 0.3f, 1);
				}
			}
			else {
				thisGem= (Gem)activeHand.GetChild(0);
				thisGem.CallDeferred("Fail");
				await ToSignal(thisGem, "FinishedTrigger");
				speedMult = Mathf.Clamp(speedMult-0.1f, 0.3f, 1);
			}
		}
		curses.Clear();

		GD.Print("end attack");
		seqPowerMult = 1;
		seqCostMult = 1;
		mult.Visible = false;
		multFX.QueueFree();

		SwitchState(Phase.EnemyChoice);
	}

	public void PrepareEnemes(Godot.Collections.Array<PackedScene> spawnEnemies) {
		theseEnemies = spawnEnemies;
		for(int i = 0; i < enemies.GetChildCount(); i++) {
			enemies.GetChild(i).QueueFree();
		}
		for(int i = 0; i < spawnEnemies.Count; i++) {
			GD.Print("spawningEnemies");
			Enemy thisEnemy = spawnEnemies[i].Instantiate() as Enemy;
			enemies.AddChild(thisEnemy);
			thisEnemy.Position = spawnPos[i];
		}

	}

	public void PrimeGem(Gem gem) {
		hand.RemoveChild(gem);
		activeHand.AddChild(gem);
		battleOptions.GetNode<Button>("Attack").Text = "ATTACK";
		if( (activeHand.GetChildCount() == 1 || activeHand.GetChildCount() == 2) && player.pocketedGems.Count == 0) {
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
		if((activeHand.GetChildCount() == 1 || activeHand.GetChildCount() == 2) && player.pocketedGems.Count == 0) {
			battleOptions.GetNode<Button>("Pocket").Disabled = false;
		} else {
			battleOptions.GetNode<Button>("Pocket").Disabled = true;
		}
	}

	public Enemy GetEnemy(int ind) {
		if(SelectedEnemyIndex == -1) SelectedEnemyIndex = 0;
		Enemy theEnemy;
		try {
			theEnemy = enemies.GetChild<Enemy>(ind);
		}
		catch (Exception ex) {
			SelectedEnemyIndex = 0;
		}
		if(ind == -1) ind = SelectedEnemyIndex;
		theEnemy = enemies.GetChild<Enemy>(ind);
		return enemies.GetChild<Enemy>(ind);
	}

	public void SetEnemySelection(int ind) {
		SelectedEnemyIndex = ind;
		reticle.GlobalPosition = enemies.GetChild<Node2D>(ind).GlobalPosition;
	}
	public void _on_try_again_pressed() {
		GetParent().GetParent().RemoveChild(GetParent());
		GetParent().CallDeferred("queue_free");
		GameController.Instance.EmitSignal("BattleEnded", false);
	}
	
}
