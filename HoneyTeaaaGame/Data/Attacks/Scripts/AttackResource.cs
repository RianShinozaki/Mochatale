using Godot;
using System;

[GlobalClass]
public partial class AttackResource : Resource
{
	[Signal]
	public delegate void AttackEndedEventHandler();
	public async virtual void Execute(Enemy enemy) {}
}
