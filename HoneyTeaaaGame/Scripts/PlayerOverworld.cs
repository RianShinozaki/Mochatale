using Godot;
using System;

public partial class PlayerOverworld : CharacterBody2D
{
	// Export refs
	[Export] float moveSpeed;

	// References
	AnimationTree animTree;
	Sprite2D sprite;

	// Member variables
	bool walking = false;

	public override void _Ready()
	{
		animTree = GetNode<AnimationTree>("Sprite2D/AnimationTree");
		sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _Process(double delta)
	{
		if(GameController.currentGameMode != GameController.GameMode.Overworld) return;

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
