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
		var tween = enemy.GetTree().CreateTween().BindNode(enemy).SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(enemy.sprite, "offset", new Vector2(-5f, 0), 0.05f);
		tween.TweenProperty(enemy.sprite, "offset", new Vector2(0, 0), 0.1f);

		await ToSignal(Battle.Instance.player, "AnimationEnded");
		EmitSignal(SignalName.AttackEnded);
	}
}
