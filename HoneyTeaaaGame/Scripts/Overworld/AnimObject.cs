using Godot;
using System;

public partial class AnimObject : Node2D
{
	[Signal]
	public delegate void EventFinishedEventHandler();
	public void Execute() {
		
	}
}
