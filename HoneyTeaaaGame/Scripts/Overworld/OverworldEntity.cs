using Godot;
using System;

public partial class OverworldEntity : CharacterBody2D
{
	// Export refs
	[Export] public float moveSpeed;

	// References
	public AnimationTree animTree;
	public Sprite2D sprite;

	// Member variables
	bool walking = false; //For animation
	bool moving = false; //For movement

	// Const
	public enum Direction {
		Up,
		Right,
		Down,
		Left
	}

	public override void _Ready()
	{
		animTree = GetNode<AnimationTree>("Sprite2D/AnimationTree");
		sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _Process(double delta)
	{
	}
	public virtual Vector2 SetDirection(Direction direction) {
		Vector2 dirVec = GetVecFromDir(direction);

		animTree.Set("parameters/Idle/blend_position", dirVec);
		animTree.Set("parameters/Walk/blend_position", dirVec);
		return dirVec;
	}
	public virtual Vector2 GetVecFromDir(Direction direction) {
		Vector2 dirVec = Vector2.Zero;

		switch(direction) {
			case Direction.Up:
				dirVec = Vector2.Up;
				break;
			case Direction.Right:
				dirVec = Vector2.Right;
				break;
			case Direction.Down:
				dirVec = Vector2.Down;
				break;
			case Direction.Left:
				dirVec = Vector2.Left;
				break;
		}
		return dirVec;
	}
	public async virtual void MoveDistance(Direction direction, int tiles) {
		//When you move, you go toMove * delta
		Vector2 moveDir = SetDirection(direction);
		float toMove = tiles*16;
		//while(toMove > 0) {
			//toMove -= moveSpeed * 
		//}
	}
}
