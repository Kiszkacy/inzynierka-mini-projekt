using Godot;
using System;

public partial class GameMaster : Node, Observable
{
    [Export]
    public bool LeftSideBot { get; set; } = true;
    
    [Export]
    public bool RightSideModel { get; set; } = false;

    private Agent leftAgent;
    private Agent rightAgent;
    
    private Paddle leftPaddle;
    private Paddle rightPaddle;
    private Ball ball;
    
    public override void _Ready()
    {
        EventManager.Get().Subscribe(this);
        EventManager.Get().RegisterEvent(new Event("RESET"));

        if (LeftSideBot)
        {
            this.leftAgent = new SimpleBot();
        }
        if (RightSideModel)
        {
            PipeHandler.Get().Connect();
            this.rightAgent = new Model(
                GetParent().GetNode<RewardHandler>("RewardHandler")
            );
        }
        if (LeftSideBot || RightSideModel)
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
        if (this.LeftSideBot)
        {
            Action action = this.leftAgent.GetAction(
                Side.Left, 
                leftPaddle.GlobalPosition.Y, 
                rightPaddle.GlobalPosition.Y, 
                ball.GlobalPosition,
                ball.Velocity
            );
            this.UpdatePaddlePositionBasedOnAction(Side.Left, action);
        }
        if (this.RightSideModel)
        {
            Action action = this.rightAgent.GetAction(
                Side.Right, 
                leftPaddle.GlobalPosition.Y, 
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