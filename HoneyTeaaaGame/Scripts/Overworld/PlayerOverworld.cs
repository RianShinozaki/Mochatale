using Godot;
using System;

public partial class PlayerOverworld : OverworldEntity
{

	// Member variables
	bool walking = false;
	public static PlayerOverworld Instance;

	public override void _Ready()
	{
		base._Ready();
		Instance = this;
	}

	public override void _Process(double delta)
	{
		if(GameController.currentGameMode != GameController.GameMode.Overworld) {
			walking = false;
			return;
		}

		float hor = Mathf.Sign(Input.GetAxis("ui_left", "ui_right"));
		float vert = Mathf.Sign(Input.GetAxis("ui_up", "ui_down"));
		Vector2 mov = new Vector2(hor, vert);
		Velocity = moveSpeed * mov;

		if(mov != Vector2.Zero) {
			animTree.Set("parameters/Idle/blend_position", mov);
			animTree.Set("parameters/Walk/blend_position", mov);
			walking = true;
			sprite.FlipH = mov.X == -1;
		} else {
			walking = false;
		}

		MoveAndSlide();
	}
}
