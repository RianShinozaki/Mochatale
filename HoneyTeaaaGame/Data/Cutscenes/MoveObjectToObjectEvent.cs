using Godot;
using System;

[GlobalClass]
public partial class MoveObjectToObjectEvent : CutsceneEvent
{
	[Export] NodePath src;
	[Export] NodePath dest;
	public async override void Execute(CutsceneTrigger caller) {
		caller.GetNode<Node2D>(src).GlobalPosition = caller.GetNode<Node2D>(dest).GlobalPosition;
	}
}
