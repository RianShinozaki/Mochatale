using Godot;
using System;

public partial class SpinningSurface : Node3D
{
	[Export] public float rotSpeed = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(this, "rotSpeed", 0.2f, 1f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Rotate(Vector3.Up, rotSpeed * (float)delta);
	}
}
