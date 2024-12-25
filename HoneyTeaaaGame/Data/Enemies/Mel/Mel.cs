using Godot;
using System;

public partial class Mel : Enemy
{
	[Export] public AttackResource myAttack;
	[Export] public DialogueBridge dialogueBridge;
	public override void _Ready()
	{
		base._Ready();
		//dialogueBridge = GetNode<DialogueBridge>("DialogueBridge");
		//dialogueBridge.StartDialogueID("Threaten");
	}

	public override void _Process(double delta)
	{
	}
	public async override void EnemyTurn() {
		base.EnemyTurn();
		
		if(!active) return;
		myAttack.Execute(this);
		await ToSignal(myAttack, "AttackEnded");
		EmitSignal(SignalName.EnemyTurnFinished);
	}
}
