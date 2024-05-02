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
	public Action GetAction(double paddlePosition, Vector2 ballPosition, Vector2 ballVelocity);
}
