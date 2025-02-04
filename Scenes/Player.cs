using Godot;
using System;
using System.Security.Cryptography;

public partial class Player : Area2D
{

	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public int Speed {get;set;} = 32;

	public Vector2 ScreenSize;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;

		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		var velocity = Vector2.Zero; //Make the player's movement vector

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_jump"))
		{
			//Put code for handling the jump here
		}


		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); 


		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}

		if (velocity.X != 0)
		{
			animatedSprite2D.Animation = "Run";
			animatedSprite2D.FlipV = false;
			animatedSprite2D.FlipH = velocity.X < 0;
		}




		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
			);


	}
}
