using Godot;
using System;

[GlobalClass]
public partial class SetDialogueEvent : CutsceneEvent
{
    [Export] public string dialoguePath;

	public async override void Execute(CutsceneTrigger caller) {
        runAsync = false;
        DialogueBridge.Instance.SwapDialogueBoxData(dialoguePath);
	}
}
