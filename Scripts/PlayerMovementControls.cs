using Godot;

public partial class PlayerMovementControls : Area2D
{
// Point of this script is to allow access to player movement, give them jumps and handle collision behavior (jumping or running into a wall)

	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public int Speed {get;set;} = 100;

	[Export]
	public Vector2 position {get;set;} = new Vector2(128, 64);


	[Export]
	public float JumpHeight {get;set;} = 150;
	
	[Export]
	public float JumpTimeToPeak {get;set;} = 0.2f;

	[Export]
	public float JumpTimeToDescent {get;set;} = 0.1f;


	public float JumpVelocity;
	public float JumpAscentGravity;
	public float JumpDescentGravity;

	public string input_right = "move_right";
	public string input_left = "move_left";
	public string input_jump = "move_jump";


	public Vector2 ScreenSize;

	private void OnBodyEntered(Node2D body)
	{
		Hide();
		EmitSignal(SignalName.Hit);
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	public void Initialize()
	{
		Position = position;
		
		JumpVelocity = (2.0f * JumpHeight / JumpTimeToPeak) * -1.0f;
		JumpAscentGravity = ((-2.0f * JumpHeight)/ (JumpTimeToPeak * JumpTimeToPeak)) * -1.0f;
		JumpDescentGravity = ((-2.0f * JumpHeight)/ (JumpTimeToDescent * JumpTimeToDescent)) * -1.0f;

		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;

	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
		Initialize();
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero; //Make the player's movement vector
		velocity.Y += GetGravity(velocity.Y) * (float)delta;

		if (Input.IsActionPressed(input_right))
		{
			velocity.X += 1;
		}

		if (Input.IsActionPressed(input_left))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed(input_jump))
		{
			//Put code for handling the jump here

			velocity.Y = JumpVelocity;
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

	private float GetGravity(float YVelocity)
	{
		if (YVelocity < 0.0f) 
		{
			return JumpAscentGravity;
		}
		else 
		{
			return JumpDescentGravity;
		}
	}

}
