using Godot;
using System;

public partial class GameController : Node
{
	[Export] public string atkDialogueResPath;
	public static GameController Instance;

	public override void _EnterTree()
	{
		Instance = this;
	}

}
