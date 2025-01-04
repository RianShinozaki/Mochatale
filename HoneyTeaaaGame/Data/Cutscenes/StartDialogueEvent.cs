using Godot;
using System;

[GlobalClass]
public partial class StartDialogueEvent : CutsceneEvent
{
    [Export] public string startID;

	public async override void Execute(CutsceneTrigger caller) {
        runAsync = true;
        DialogueBridge.Instance.StartDialogueID(startID);
		await ToSignal(DialogueBridge.Instance.dialogueBox, "dialogue_ended");

		EmitSignal(SignalName.EventFinished);
	}
}
