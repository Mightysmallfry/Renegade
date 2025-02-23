using Godot;

public partial class PlayerMovementController : CharacterBody2D
{
	[Export] public float MoveSpeed = 300.0f;
	[Export] public float Acceleration = 100.0f;
	[Export] public float AirControlFactor = 1.0f;
	[Export] public float Friction = 50.0f;
	[Export] public float JumpForce = -1200.0f;
	[Export] public float MaxFallSpeed = 800.0f;
	[Export] public float Gravity = 100.0f;
	[Export] public float FallGravityMultiplier = 2.2f; // Faster falling
	[Export] public float CoyoteTime = 0.1f;
	[Export] public float JumpBufferTime = 0.1f;
	[Export] public Vector2 StartingPosition = Vector2.Zero;

	private float coyoteTimeCounter = 0;
	private float jumpBufferCounter = 0;
	private bool isJumping = false;
	private Vector2 velocity;
	
	private AnimatedSprite2D animatedSprite2D;

    public override void _Ready()
    {
        base._Ready();
		Position = StartingPosition;
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Show();
    }

    public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		// Get input direction (-1 for left, 1 for right, 0 if no input)
		float direction = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		//velocity = Vector2.Zero;

		// Apply Gravity
		if (!IsOnFloor())
		{
			velocity.Y += (velocity.Y > 0 ? Gravity * FallGravityMultiplier : Gravity);
			velocity.Y = Mathf.Min(velocity.Y, MaxFallSpeed);
		}
		else
		{
			coyoteTimeCounter = CoyoteTime; // Reset coyote time
			velocity.Y = 0.0f;
		}

		// Handle Jump Buffering
		if (Input.IsActionJustPressed("move_jump"))
		{
			jumpBufferCounter = JumpBufferTime;
		}
		else
		{
			jumpBufferCounter -= dt;
		}

		// Jumping Logic
		if (jumpBufferCounter > 0 && (IsOnFloor() || coyoteTimeCounter > 0))
		{
			velocity.Y = JumpForce;
			isJumping = true;
			jumpBufferCounter = 0;
			coyoteTimeCounter = 0; // Prevent double jumping using coyote time
		}

		// Cut jump height if button is released early
		if (Input.IsActionJustReleased("move_jump") && isJumping && velocity.Y < 0)
		{
			velocity.Y *= 0.5f; // Makes short jumps feel responsive
		}


		// Acceleration & Friction (Snappy Controls)
		float targetSpeed = direction * MoveSpeed;
		float acceleration = IsOnFloor() ? Acceleration : Acceleration * AirControlFactor;
		if (direction != 0)
		{
			velocity.X = Mathf.MoveToward(velocity.X, targetSpeed, acceleration);
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, Friction);
		}

		ProcessAnimations(velocity);

		// Apply Movement
		Velocity = velocity;
		MoveAndSlide();

		// Reduce coyote time each frame
		coyoteTimeCounter -= dt;
	}



	private void ProcessAnimations(Vector2 velocity)
	{
		if (velocity.Length() > 0)
		{
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
		if (velocity.Y != 0)
		{
			animatedSprite2D.Animation = "Jump";
			animatedSprite2D.FlipV = false;
		}

	}
}
