using Godot;
using System;

public partial class GameMaster : Node, Observable
{
    public override void _Ready()
    {
        EventManager.Get().Subscribe(this);
        EventManager.Get().RegisterEvent(new Event("RESET"));
    }
	
    public override void _Process(double delta)
    {
		
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