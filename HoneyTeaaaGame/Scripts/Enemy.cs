using Godot;
using System;

public partial class Enemy : Node2D
{
	[Export] public float HP;
	[Export] public float maxHP;
	ProgressBar progBar;
	public AnimationPlayer anim;
	public Label Damage;
    public override void _Ready()
    {
        anim = GetNode<Sprite2D>("HitFX").GetNode<AnimationPlayer>("AnimationPlayer");
		progBar = GetNode<ProgressBar>("ProgressBar");
		HP = maxHP;
		Damage = GetNode<Label>("Label");
    }
    public void TakeDamage(float damage) {
		anim.Play("default");
		anim.Play("Hit");
		HP -= damage;
		progBar.Value = HP/maxHP;
		GD.Print(HP);
		//Damage.Visible = true;
		//Damage.Text = "-" + damage.ToString("");
	}
}