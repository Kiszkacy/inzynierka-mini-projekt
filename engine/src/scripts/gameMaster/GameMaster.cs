using Godot;


public enum PaddleControl
{
	Player,
	Bot,
	Model
}

public partial class GameMaster : Node, Observable
{
	[Export]
	public PaddleControl LeftPaddleControl { get; set; } = PaddleControl.Bot;
	
	[Export]
	public PaddleControl RightPaddleControl { get; set; } = PaddleControl.Player;

	private Agent leftAgent;
	private Agent rightAgent;
	
	private Paddle leftPaddle;
	private Paddle rightPaddle;
	private Ball ball;
	
	public override void _Ready()
	{
		EventManager.Get().Subscribe(this);
		EventManager.Get().RegisterEvent(new Event("RESET"));
		
		if (LeftPaddleControl == PaddleControl.Model && RightPaddleControl == PaddleControl.Model){
			GD.Print("Model can't be set on both sides. Changing left paddle to bot.");
			LeftPaddleControl = PaddleControl.Bot;
		}

		if (LeftPaddleControl == PaddleControl.Bot)
		{
			this.leftAgent = new SimpleBot();
		}
		else if (LeftPaddleControl == PaddleControl.Model){
			PipeHandler.Get().Connect();
			this.leftAgent = new Model();
		}
		if (RightPaddleControl == PaddleControl.Bot)
		{
			this.rightAgent = new SimpleBot();
		}
		else if (RightPaddleControl == PaddleControl.Model){
			PipeHandler.Get().Connect();
			this.rightAgent = new Model();
		}
		
		if (!(LeftPaddleControl == PaddleControl.Player && RightPaddleControl == PaddleControl.Player))
		{
			this.leftPaddle = GetParent().GetNode<Paddle>("LeftPaddle");
			this.rightPaddle = GetParent().GetNode<Paddle>("RightPaddle");
			this.ball = GetParent().GetNode<Ball>("Ball");
		}
	}
	
	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		if (LeftPaddleControl != PaddleControl.Player)
		{
			Action action = this.leftAgent.GetAction(
				leftPaddle.GlobalPosition.Y,
				ball.GlobalPosition,
				ball.Velocity
			);
			this.UpdatePaddlePositionBasedOnAction(Side.Left, action);
		}
		if (RightPaddleControl != PaddleControl.Player)
		{
			Action action = this.rightAgent.GetAction(
				rightPaddle.GlobalPosition.Y, 
				ball.GlobalPosition,
				ball.Velocity
			);
			this.UpdatePaddlePositionBasedOnAction(Side.Right, action);
		}
	}

	private void UpdatePaddlePositionBasedOnAction(Side side, Action action)
	{
		Paddle paddle = side == Side.Left ? this.leftPaddle : this.rightPaddle;
		switch (action)
		{
			case Action.Up:
				paddle.StopDown();
				paddle.GoUp();
				break;
			case Action.Down:
				paddle.StopUp();
				paddle.GoDown();
				break;
			case Action.Stop:
				paddle.StopUp();
				paddle.StopDown();
				break;
		}
	}

	public void Notify(Event @event)
	{
		if (@event.Code == "SIDE.LEFT.SCORE")
		{
			this.ResetBall(Side.Left);
		}
		else if (@event.Code == "SIDE.RIGHT.SCORE")
		{
			this.ResetBall(Side.Right);
		}
		else if (@event.Code == "SIDE.LEFT.WON" || @event.Code == "SIDE.RIGHT.WON")
		{
			this.ResetGame();
		}
		else if (@event.Code == "RESET.BALL.REQUEST")
		{
			this.ResetBall(Side.Left);
		}
	}

	private void ResetBall(Side launchSide)
	{
		if (launchSide == Side.Left)
		{
			EventManager.Get().RegisterEvent(new Event("BALL.LAUNCH.LEFT"));
		}
		else if (launchSide == Side.Right)
		{
			EventManager.Get().RegisterEvent(new Event("BALL.LAUNCH.RIGHT"));
		}
	}

	private void ResetGame()
	{
		EventManager.Get().RegisterEvent(new Event("RESET"));
	}

	public GameMaster()
	{
		
	}
}
