using Godot;

public partial class CameraController : Camera2D
{
    [Export] public NodePath TargetPath; // Assign this in the editor or change dynamically
    [Export] public float FollowSpeed = 5.0f; // Adjust for smoothness
    [Export] public Vector2 Offset = new Vector2(0, -50); // Adjust for better framing
    [Export] public bool UseBounds = false;
    [Export] public Vector2 MinBounds = new Vector2(-500, -500);
    [Export] public Vector2 MaxBounds = new Vector2(500, 500);

    private Node2D target; // The object the camera follows

    public override void _Ready()
    {
        if (TargetPath != null)
        {
            target = GetNode<Node2D>(TargetPath);
        }
    }

    public override void _Process(double delta)
    {
        if (target == null) return;

        // Target position with offset
        Vector2 targetPosition = target.GlobalPosition + Offset;

        // Smoothly move towards the target
        Position = Position.Lerp(targetPosition, (float)delta * FollowSpeed);

        // Clamp the camera position if bounds are enabled
        if (UseBounds)
        {
            Position = new Vector2(
                Mathf.Clamp(Position.X, MinBounds.X, MaxBounds.X),
                Mathf.Clamp(Position.Y, MinBounds.Y, MaxBounds.Y)
            );
        }
    }

    // Function to dynamically change the camera target
    public void SetTarget(Node2D newTarget)
    {
        target = newTarget;
    }
}