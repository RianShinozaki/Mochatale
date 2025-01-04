using Godot;
using System;

[GlobalClass]
public partial class CutsceneEvent : Resource
{
	[Signal]
	public delegate void EventFinishedEventHandler();
	public bool runAsync = false;
	public async virtual void Execute(CutsceneTrigger caller) {

	}
}
