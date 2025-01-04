using Godot;
using Microsoft.VisualBasic;
using System;

[GlobalClass]
public partial class SpeakAttack : AttackResource
{
	String DialogueID = "CustomAttack";
	[Export] String customText = "";
	RandomNumberGenerator rand;
	
	public override async void Execute(Enemy enemy) {
		rand = new RandomNumberGenerator();
		
		DialogueBridge.Instance.SwapDialogueBoxData(GameController.atkDialogueResPath);
		DialogueBridge.Instance.SetVariable("EnemyName", 0, enemy.name);
		DialogueBridge.Instance.SetVariable("Text", 0, customText);
		DialogueBridge.Instance.StartDialogueID(DialogueID);

		await ToSignal(DialogueBridge.Instance.dialogueBox, "dialogue_ended");

		EmitSignal(SignalName.AttackEnded);
	}
}
