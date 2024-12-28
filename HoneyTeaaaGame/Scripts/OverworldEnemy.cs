using Godot;
using System;

public partial class OverworldEnemy : CharacterBody2D
{
	[Export] public EnemySet myEnemySet;
	public override void _Ready()
	{
		GetNode<Area2D>("Hitbox").BodyEntered += _BodyEntered;
	}

	public void _BodyEntered(Node2D body) {
		if(GameController.currentGameMode != GameController.GameMode.Overworld) return;
		GameController.Instance.BattleEnded += _onBattleEnded;
		GameController.StartBattle(myEnemySet);
	}
	public void _onBattleEnded(bool won) {
		if(!won) {
			GameController.StartBattle(myEnemySet);
		}
		else {
			QueueFree();
		}
	}
}
