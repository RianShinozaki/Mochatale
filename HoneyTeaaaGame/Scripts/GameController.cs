using Godot;
using System;

public partial class GameController : Node
{
	[Export] public bool godMode = false;
	public enum GameMode {
		Overworld,
		Battle,
		Menu,
		Cutscene
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
		OverworldData.Instance.playbackOffset = MusicController.music.audioStreamPlayer.GetPlaybackPosition();
		MusicController.StartMusic(enemySet.battleMusic, 0, enemySet.battleMusicVolume);

		currentGameMode = GameMode.Battle;
		battleNode = battleScene.Instantiate() as Node;
		Instance.AddChild(battleNode);
		battleNode.GetNode<Battle>("Battle").PrepareBattle(enemySet);
		battleNode.GetNode<Battle>("Battle").SwitchState(Battle.Phase.BattleIntro);
		DialogueBridge.Instance.dialogueBox = battleNode.GetNode<Control>("Battle/DialogueBox");

	}
	public static void EndBattle(bool won) {
		DialogueBridge.Instance.dialogueBox = Instance.GetNode<Control>("CanvasLayer/DialogueBox");
		currentGameMode = GameMode.Overworld;
		if(won) {
			MusicController.StartMusic(OverworldData.Instance.overworldTrack, 0.5f, 0f, OverworldData.Instance.playbackOffset);
		}
		Instance.EmitSignal(SignalName.BattleEnded, won);
	}
	public static void LoadOverworld(PackedScene overworld) {
		if(Instance.GetNode<OverworldData>("Overworld") != null) {
			Instance.GetNode<OverworldData>("Overworld").QueueFree();
		}
		OverworldData newOverworld = overworld.Instantiate() as OverworldData;
		Instance.CallDeferred(MethodName.AddChild, newOverworld);
	}

}
