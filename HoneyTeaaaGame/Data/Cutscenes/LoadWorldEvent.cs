using Godot;
using System;

[GlobalClass]
public partial class LoadWorldEvent : CutsceneEvent
{
	[Export] public PackedScene overworld;
	public async override void Execute(CutsceneTrigger caller) {
        runAsync = false;
		GameController.LoadOverworld(overworld);
	}
}
