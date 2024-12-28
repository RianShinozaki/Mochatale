using Godot;
using Microsoft.VisualBasic;
using System;

[GlobalClass]
public partial class BasicAttack : AttackResource
{
	[Export] int DamageValue;
	[Export] int Range;
	[Export] String DialogueID;
	[Export] String customText = "";
	RandomNumberGenerator rand;
	
	public override async void Execute(Enemy enemy) {
		rand = new RandomNumberGenerator();
		
		DialogueBridge.Instance.SwapDialogueBoxData(GameController.atkDialogueResPath);
		DialogueBridge.Instance.SetVariable("EnemyName", 0, enemy.name);
		DialogueBridge.Instance.SetVariable("Text", 0, customText);
		DialogueBridge.Instance.StartDialogueID(DialogueID);

		await ToSignal(DialogueBridge.Instance.dialogueBox, "dialogue_ended");

		Battle.Instance.player.ChangeHP(-DamageValue + rand.RandiRange(-Range, Range));
		var tween = enemy.GetTree().CreateTween().BindNode(enemy).SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(enemy.sprite, "offset", new Vector2(-5f, 0), 0.05f);
		tween.TweenProperty(enemy.sprite, "offset", new Vector2(0, 0), 0.1f);

		await ToSignal(Battle.Instance.player, "AnimationEnded");
		EmitSignal(SignalName.AttackEnded);
	}
}
