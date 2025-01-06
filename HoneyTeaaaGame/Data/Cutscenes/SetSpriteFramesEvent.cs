using Godot;
using System;

[GlobalClass]
public partial class SetSpriteFramesEvent : CutsceneEvent
{
	[Export] NodePath animatedSprite2D;
	[Export] string animationName;
	public async override void Execute(CutsceneTrigger caller) {
		runAsync = false;
		caller.GetNode<AnimatedSprite2D>(animatedSprite2D).Play(animationName);
	}
}
