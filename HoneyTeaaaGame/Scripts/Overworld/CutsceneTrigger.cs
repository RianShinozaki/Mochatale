using Godot;
using System;

public partial class CutsceneTrigger : Area2D
{
	[Export] public Godot.Collections.Array<CutsceneEvent> cutsceneEvents;
	[Export] public bool destroy;
	public override void _Ready()
	{
		BodyEntered += _BodyEntered;
	}
    public override void _PhysicsProcess(double delta)
    {
        base._Process(delta);
    }

    public void _BodyEntered(Node2D body) {
		if(GameController.currentGameMode != GameController.GameMode.Overworld) return;
		CutsceneManager.Instance.CutsceneFinished += _onCutsceneEnded;
		CutsceneManager.Instance.ExecuteCutscene(this, cutsceneEvents);
	}
	public void _onCutsceneEnded() {
		CutsceneManager.Instance.CutsceneFinished -= _onCutsceneEnded;
		if(destroy)
			QueueFree();
	}
}
