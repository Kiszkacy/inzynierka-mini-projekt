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
	
	[Export] 
	public bool ShowScoreboard { get; set; } = true;
	
	private Label ScoreLeftLabel { get; set; }
	private Label ScoreRightLabel { get; set; }
	
	public override void _Ready()
	{
		EventManager.Get().Subscribe(this);
		if (this.ShowScoreboard)
		{
			this.ScoreLeftLabel = GetNode<Label>("Root/Rows/ScoreRow/ScoreLeft");
			this.ScoreRightLabel = GetNode<Label>("Root/Rows/ScoreRow/ScoreRight");
			this.UpdateLabels();
		}
		else
		{
			Control root = GetNode<Control>("Root");
			root.Visible = false;
			root.ProcessMode = ProcessModeEnum.Disabled;
		}
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
		if (this.ShowScoreboard)
		{
			this.UpdateLabels();
		}
	}

	public void IncreaseScoreRight()
	{
		this.ScoreRight += 1;
		this.CheckIfWon(Side.Right);
		if (this.ShowScoreboard)
		{
			this.UpdateLabels();
		}
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

	private void UpdateLabels()
	{
		this.ScoreLeftLabel.Text = this.ScoreLeft.ToString();
		this.ScoreRightLabel.Text = this.ScoreRight.ToString();
	}

	private void ResetScores()
	{
		this.ScoreLeft = 0;
		this.ScoreRight = 0;
		if (this.ShowScoreboard)
		{
			this.UpdateLabels();
		}
	}

	public ScoreHandler()
	{
		
	}
}
