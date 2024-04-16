
using Godot;

public class SimpleBot : Agent
{
    private readonly double stopThreshold = 10.0f;
    
    public Action GetAction(Side side, double leftPaddlePosition, double rightPaddlePosition, Vector2 ballPosition, Vector2 ballVelocity)
    {
        double paddlePosition = side == Side.Left ? leftPaddlePosition : rightPaddlePosition;

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