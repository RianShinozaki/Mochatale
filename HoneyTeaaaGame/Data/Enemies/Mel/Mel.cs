using Godot;
using System;

public partial class Mel : Enemy
{
	[Export] public AttackResource myAttack;
	public override void _Ready()
	{
		base._Ready();
	}

	public override void _Process(double delta)
	{
	}
	public async override void EnemyTurn() {
		myAttack.Execute(this);
		await ToSignal(myAttack, "AttackEnded");
		EmitSignal(SignalName.EnemyTurnFinished);
	}
}
