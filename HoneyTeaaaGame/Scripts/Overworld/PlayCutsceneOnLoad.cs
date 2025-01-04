using Godot;
using System;

public partial class PlayCutsceneOnLoad : CutsceneTrigger
{
	public override void _Ready()
	{
		CutsceneManager.Instance.ExecuteCutscene(this, cutsceneEvents);
		
	}
	public override void _Process(double delta)
	{
	}
}
