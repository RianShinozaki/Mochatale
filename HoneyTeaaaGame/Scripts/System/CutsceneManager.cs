using Godot;
using System;

public partial class CutsceneManager : Node
{
	[Signal]
	public delegate void CutsceneFinishedEventHandler();

	public static CutsceneManager Instance;
    public override void _Ready()
    {
		Instance = this;
        base._Ready();
    }

    public async void ExecuteCutscene(CutsceneTrigger caller, Godot.Collections.Array<CutsceneEvent> events) {
		GameController.currentGameMode = GameController.GameMode.Cutscene;
		for (int i = 0; i < events.Count; i++) {

			events[i].Execute(caller);
			if(events[i].runAsync) {
				await ToSignal(events[i], "EventFinished");
			}
		}
		EmitSignal(SignalName.CutsceneFinished);
		GameController.currentGameMode = GameController.GameMode.Overworld;
	}

}
