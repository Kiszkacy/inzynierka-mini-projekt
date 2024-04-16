using Godot;
using System;

public partial class ScoreArea : Area2D
{
	[Export]
	public Side Side { get; set; }
	
	public override void _Ready()
	{
		
	}
	
	public override void _Process(double delta)
	{
		
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body is not Ball _) return;
		
		EventManager.Get().RegisterEvent(new Event(this.Side.GetCode()+".SCORE"));
	}

	public ScoreArea()
	{
		
	}
}
