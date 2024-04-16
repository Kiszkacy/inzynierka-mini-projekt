using Godot;
using System;

public enum Side
{
	Left,
	Right,
	None
}

public static class SideExtensions
{
	public static string GetCode(this Side side)
	{
		return side switch
		{
			Side.Left => "SIDE.LEFT",
			Side.Right => "SIDE.RIGHT",
			_ => string.Empty
		};
	}
}

public partial class ScoreHandler : Node, Observable
{
	[Export] 
	public int MaxScore { get; set; } = 5;
	
	public int ScoreLeft { get; set; }
	public int ScoreRight { get; set; }
	
	public override void _Ready()
	{
		EventManager.Get().Subscribe(this);
	}

	public override void _Process(double delta)
	{
		
	}
	
	public void Notify(Event @event)
	{
		if (@event.Code == "SIDE.LEFT.SCORE")
		{
			this.IncreaseScoreLeft();
		}
		else if (@event.Code == "SIDE.RIGHT.SCORE")
		{
			this.IncreaseScoreRight();
		}
		else if (@event.Code == "RESET")
		{
			this.ResetScores();
		}
	}

	public void IncreaseScoreLeft()
	{
		this.ScoreLeft += 1;
		this.CheckIfWon(Side.Left);
	}

	public void IncreaseScoreRight()
	{
		this.ScoreRight += 1;
		this.CheckIfWon(Side.Right);
	}
	
	public void IncreaseScore(Side side)
	{
		if (side == Side.Left)
		{
			this.IncreaseScoreLeft();
		}
		else if (side == Side.Right)
		{
			this.IncreaseScoreRight();
		}
	}

	private void CheckIfWon(Side side)
	{
		if (side == Side.Left && this.ScoreLeft == this.MaxScore)
		{
			EventManager.Get().RegisterEvent(new Event("SIDE.LEFT.WON"));
		}
		else if (side == Side.Right && this.ScoreRight == this.MaxScore)
		{
			EventManager.Get().RegisterEvent(new Event("SIDE.RIGHT.WON"));
		}
	}

	private void ResetScores()
	{
		this.ScoreLeft = 0;
		this.ScoreRight = 0;
	}

	public ScoreHandler()
	{
		
	}
}
