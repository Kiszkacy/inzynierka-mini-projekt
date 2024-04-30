using Godot;
using System;

public partial class Initializer : Node
{
	public override void _Ready()
	{	
		Engine.TimeScale = 3.0;
		Engine.PhysicsTicksPerSecond = 60;
		GD.Print("OK!");
	}
	
	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		// TODO this should be called in a special class that runs logic at the end of each godot frame
		EventManager.Get().EmitDelayedEvents();
	}

	public Initializer()
	{
		
	}
}
