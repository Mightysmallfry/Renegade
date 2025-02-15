using Godot;

public partial class PlayerMovementController : CharacterBody2D
{
	[Export] public float MoveSpeed = 200.0f;
	[Export] public float Acceleration = 20.0f;
	[Export] public float AirControlFactor = 0.6f;
	[Export] public float Friction = 15.0f;
	[Export] public float JumpForce = -400.0f;
	[Export] public float MaxFallSpeed = 900.0f;
	[Export] public float Gravity = 1200.0f;
	[Export] public float FallGravityMultiplier = 2.2f; // Faster falling
	[Export] public float CoyoteTime = 0.1f;
	[Export] public float JumpBufferTime = 0.1f;

	private float coyoteTimeCounter = 0;
	private float jumpBufferCounter = 0;
	private bool isJumping = false;
	private Vector2 velocity;

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		// Get input direction (-1 for left, 1 for right, 0 if no input)
		float direction = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");

		// Apply Gravity
		if (!IsOnFloor())
		{
			velocity.Y += (velocity.Y > 0 ? Gravity * FallGravityMultiplier : Gravity) * dt;
			velocity.Y = Mathf.Min(velocity.Y, MaxFallSpeed);
		}
		else
		{
			coyoteTimeCounter = CoyoteTime; // Reset coyote time
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
			velocity.X = Mathf.MoveToward(velocity.X, targetSpeed, acceleration * dt);
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, Friction * dt);
		}

		// Apply Movement
		Velocity = velocity;
		MoveAndSlide();

		// Reduce coyote time each frame
		coyoteTimeCounter -= dt;
	}
}
