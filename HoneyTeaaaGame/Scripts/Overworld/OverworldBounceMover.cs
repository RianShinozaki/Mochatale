using Godot;
using System;

public partial class OverworldBounceMover : OverworldEntityMover
{
	OverworldEntity entity;
	Vector2 speed;
	[Export] float maxSpeed;
	[Export] float decel;
	[Export] float waitToMove;
	float waitTimer;
	RandomNumberGenerator rand;
	public override void _Ready()
	{
		entity = GetParent<OverworldEntity>();
		rand = new RandomNumberGenerator();
	}

	public override void _Process(double delta)
	{
		float floatDelta = (float)delta;
		if(GameController.currentGameMode != GameController.GameMode.Overworld) return;

		if(speed.IsEqualApprox(Vector2.Zero)) {
			waitTimer += floatDelta;
			if(waitTimer > waitToMove) {
				speed = entity.GetVecFromDir( (OverworldEntity.Direction)rand.RandiRange(0, 3)) * maxSpeed;
				waitTimer = 0;
			}
		} else {
			speed = new Vector2(Mathf.MoveToward(speed.X, 0, decel*floatDelta), Mathf.MoveToward(speed.Y, 0, decel*floatDelta));
			entity.Velocity = speed;
			entity.MoveAndSlide();
		}
		
	}
}
