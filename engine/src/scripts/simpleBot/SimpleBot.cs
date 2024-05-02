
using Godot;

public class SimpleBot : Agent
{
	private readonly double stopThreshold = 10.0f;
	
	public Action GetAction(double paddlePosition, Vector2 ballPosition, Vector2 ballVelocity)
	{

		if (Mathf.Abs(paddlePosition - ballPosition.Y) <= this.stopThreshold)
		{
			return Action.Stop;
		}
		if (paddlePosition < ballPosition.Y)
		{
			return Action.Down;
		}
		if (paddlePosition > ballPosition.Y)
		{
			return Action.Up;
		}

		return Action.None;
	}
}
