using Godot;

public partial class Scoreboard : Node
{
	[Export] 
	public int MaxScore { get; set; } = 5;

	[Export] 
	public bool ShowScoreboard { get; set; } = true;
	
	private Label ScoreLeftLabel { get; set; }
	private Label ScoreRightLabel { get; set; }
	
	public override void _Ready()
	{
		ScoreHandler.Get().MaxScore = this.MaxScore;
		if (this.ShowScoreboard)
		{
			ScoreHandler.Get().ScoreLeft.OnChange += OnScoreLeftChange;
			ScoreHandler.Get().ScoreRight.OnChange += OnScoreRightChange;
			this.ScoreLeftLabel = GetNode<Label>("Root/Rows/ScoreRow/ScoreLeft");
			this.ScoreRightLabel = GetNode<Label>("Root/Rows/ScoreRow/ScoreRight");
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

	private void OnScoreLeftChange(object emitter, ValueChanged<int> valueChanged)
	{
		this.ScoreLeftLabel.Text = valueChanged.NewValue.ToString();
	}
	
	private void OnScoreRightChange(object emitter, ValueChanged<int> valueChanged)
	{
		this.ScoreRightLabel.Text = valueChanged.NewValue.ToString();
	}

	public Scoreboard()
	{
		
	}
}
