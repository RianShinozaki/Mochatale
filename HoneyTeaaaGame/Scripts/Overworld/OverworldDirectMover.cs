using Godot;
using System;

public partial class OverworldDirectMover : OverworldEntityMover
{
	OverworldEntity entity;
	Vector2 speed;
	[Export] float maxSpeed;
	[Export] float decel;
	[Export] float accel;
	public override void _Ready()
	{
		entity = GetParent<OverworldEntity>();
	}

	public override void _Process(double delta)
	{
		float floatDelta = (float)delta;
		if(GameController.currentGameMode != GameController.GameMode.Overworld) return;
		if(GlobalPosition.DistanceTo(PlayerOverworld.Instance.GlobalPosition) < 16*10) {
			speed += accel * floatDelta * (PlayerOverworld.Instance.GlobalPosition - GlobalPosition).Normalized();
		} else {
			speed = new Vector2(Mathf.MoveToward(speed.X, 0, decel * floatDelta), Mathf.MoveToward(speed.Y, 0, decel * floatDelta));
		}
		entity.Velocity = speed;
		entity.MoveAndSlide();
		
	}
}
