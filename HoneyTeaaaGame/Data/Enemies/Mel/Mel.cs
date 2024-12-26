using Godot;
using System;

public partial class Mel : Enemy
{
	[Export] public AttackResource basicAttack;
	[Export] public AttackResource curseAttack;
	[Export] public DialogueBridge dialogueBridge;
	public override void _Ready()
	{
		base._Ready();
	}

	public async override void EnemyTurn() {
		base.EnemyTurn();
		
		if(!active) return;
		RandomNumberGenerator rand = new RandomNumberGenerator();
		if(rand.RandiRange(1,3) == 2) {
			curseAttack.Execute(this);
			await ToSignal(curseAttack, "AttackEnded");
			EmitSignal(SignalName.EnemyTurnFinished);
		} else {
			basicAttack.Execute(this);
			await ToSignal(basicAttack, "AttackEnded");
			EmitSignal(SignalName.EnemyTurnFinished);
		}

		
	}
}
