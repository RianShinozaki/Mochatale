using Godot;
using System;

public partial class GameController : Node
{
	public enum GameMode {
		Overworld,
		Battle,
		Menu
	}
	[Signal]
	public delegate void BattleEndedEventHandler(bool won);
	public static string atkDialogueResPath = "res://Data/Attacks/AttackDialogue.tres";
	public static string battleScenePath = "res://Scenes/Battle.tscn";
	public static PackedScene battleScene;
	public static Node battleNode;
	public static GameController Instance;

	public static GameMode currentGameMode = GameMode.Overworld;

	public override void _EnterTree()
	{
		Instance = this;
		battleScene = GD.Load<PackedScene>(battleScenePath);
	}
	public static void StartBattle(EnemySet enemySet) {
		currentGameMode = GameMode.Battle;
		battleNode = battleScene.Instantiate() as Node;
		Instance.AddChild(battleNode);
		battleNode.GetNode<Battle>("Battle").PrepareEnemes(enemySet.enemies);
		
		battleNode.GetNode<Battle>("Battle").SwitchState(Battle.Phase.BattleIntro);

	}

}
