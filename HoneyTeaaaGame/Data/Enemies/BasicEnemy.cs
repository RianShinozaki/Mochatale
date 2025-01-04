using Godot;
using System;

public partial class BasicEnemy : Enemy
{
	[Export] public Godot.Collections.Array<AttackResource> attacks;
	[Export] public DialogueBridge dialogueBridge;
	[Export] public int[] moveRatios;
	public Godot.Collections.Array<AttackResource> movePool;
	RandomNumberGenerator rand;

	public override void _Ready()
	{
		rand = new RandomNumberGenerator();
		movePool = new Godot.Collections.Array<AttackResource>();
		base._Ready();
		for(int i = 0; i < attacks.Count; i++) { 
			for(int ii = 0; ii < moveRatios[i]; ii++) {
				movePool.Add(attacks[i]);
			}
		}
	}

	public async override void EnemyTurn() {
		base.EnemyTurn();
		
		if(!active) return;
		int randInt = rand.RandiRange(0, movePool.Count-1);
		
		AttackResource theAttack = movePool[randInt];
		theAttack.Execute(this);
		await ToSignal(theAttack, "AttackEnded");
		EmitSignal(SignalName.EnemyTurnFinished);
		
	}
}
