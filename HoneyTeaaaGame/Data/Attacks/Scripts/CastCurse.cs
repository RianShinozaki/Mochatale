using Godot;
using Microsoft.VisualBasic;
using System;

[GlobalClass]
public partial class CastCurse : AttackResource
{
	[Export] String DialogueID;
	[Export] Curse myCurse;
	[Export] String customText = "";

	RandomNumberGenerator rand;
	
	public override async void Execute(Enemy enemy) {
		
		DialogueBridge.Instance.SwapDialogueBoxData(GameController.atkDialogueResPath);
		DialogueBridge.Instance.SetVariable("EnemyName", 0, enemy.name);
		DialogueBridge.Instance.SetVariable("Text", 0, customText);
		DialogueBridge.Instance.StartDialogueID(DialogueID);

		await ToSignal(DialogueBridge.Instance.dialogueBox, "dialogue_ended");

		Battle.Instance.player.GetCursed(myCurse);
		var tween = enemy.GetTree().CreateTween().BindNode(enemy).SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(enemy.sprite, "scale", new Vector2(6.5f, 6.5f), 0.05f);
		tween.TweenProperty(enemy.sprite, "scale", new Vector2(6.0f, 6.0f), 0.1f);

		//await ToSignal(Battle.Instance.player, "AnimationEnded");
		
		Battle.Instance.curses.Add(myCurse);
		EmitSignal(SignalName.AttackEnded);
	}
}
