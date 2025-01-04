using Godot;
using System;

[GlobalClass]
public partial class StartBattleEvent : CutsceneEvent
{
	[Export] public EnemySet enemySet;
	public async override void Execute(CutsceneTrigger caller) {
        runAsync = true;
		GameController.Instance.BattleEnded += _onBattleEnded;
		GameController.StartBattle(enemySet);
	}
	public void _onBattleEnded(bool won) {
		if(!won) {
			GameController.StartBattle(enemySet);
		}
		else {
			GameController.Instance.BattleEnded -= _onBattleEnded;
			GameController.currentGameMode = GameController.GameMode.Cutscene;
			EmitSignal(SignalName.EventFinished);
			
		}
	}
}
