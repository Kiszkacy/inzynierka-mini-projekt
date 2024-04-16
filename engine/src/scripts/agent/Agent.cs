using Godot;

public enum Action
{
    Up,
    Down,
    Stop,
    None
}

public interface Agent
{
    public Action GetAction(Side side, double leftPaddlePosition, double rightPaddlePosition, Vector2 ballPosition, Vector2 ballVelocity);
}