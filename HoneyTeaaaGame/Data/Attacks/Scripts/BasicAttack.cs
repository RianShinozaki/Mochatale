using Godot;
using Microsoft.VisualBasic;
using System;

[GlobalClass]
public partial class BasicAttack : AttackResource
{
	[Export] int DamageValue;
	[Export] int Range;
	[Export] String DialogueID;
	RandomNumberGenerator rand;
	
	public override async void Execute(Enemy enemy) {
		rand = new RandomNumberGenerator();
		GD.Print("Starting Dialogue");

		
		DialogueBridge.Instance.SwapDialogueBoxData(GameController.Instance.atkDialogueResPath);
		DialogueBridge.Instance.SetVariable("EnemyName", 0, enemy.name);
		DialogueBridge.Instance.StartDialogueID(DialogueID);

		await ToSignal(DialogueBridge.Instance.dialogueBox, "dialogue_ended");
		Battle.Instance.player.ChangeHP(-DamageValue + rand.RandiRange(-Range, Range));

		await ToSignal(Battle.Instance.player, "AnimationEnded");
		EmitSignal(SignalName.AttackEnded);
	}
}
