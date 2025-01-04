using Godot;
using System;

[GlobalClass]
public partial class AnimationEvent : CutsceneEvent
{
	[Export] NodePath animPlayer;
    [Export] string animationName;
	public async override void Execute(CutsceneTrigger caller) {
        runAsync = true;
        caller.GetNode<AnimationPlayer>(animPlayer).Play(animationName);
        await ToSignal(caller.GetNode<AnimationPlayer>(animPlayer), "animation_finished");
        EmitSignal(SignalName.EventFinished);
	}
}
