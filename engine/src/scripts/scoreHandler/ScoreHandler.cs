
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

public class ScoreHandler : Singleton<ScoreHandler>, Observable
{
	public int MaxScore { get; set;  } = 5;

	public ObservableValue<int> ScoreLeft { get; } = new(0);
	
	public ObservableValue<int> ScoreRight { get; } = new(0);

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
		this.ScoreLeft.Value += 1;
		this.CheckIfWon(Side.Left);
	}

	public void IncreaseScoreRight()
	{
		this.ScoreRight.Value += 1;
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
		if (side == Side.Left && this.ScoreLeft.Value == this.MaxScore)
		{
			EventManager.Get().RegisterEvent(new Event("SIDE.LEFT.WON"));
		}
		else if (side == Side.Right && this.ScoreRight.Value == this.MaxScore)
		{
			EventManager.Get().RegisterEvent(new Event("SIDE.RIGHT.WON"));
		}
	}

	private void ResetScores()
	{
		this.ScoreLeft.Value = 0;
		this.ScoreRight.Value = 0;
	}

	private ScoreHandler()
	{
		EventManager.Get().Subscribe(this);
	}
}
