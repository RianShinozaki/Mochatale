using Godot;
using System;

[GlobalClass]
public partial class WaitEvent : CutsceneEvent
{
    [Export] public float time;

	public async override void Execute(CutsceneTrigger caller) {
        runAsync = true;
        SceneTreeTimer timer = CutsceneManager.Instance.GetTree().CreateTimer(time);
        await ToSignal(timer, "timeout");
        EmitSignal(SignalName.EventFinished);
	}
}
