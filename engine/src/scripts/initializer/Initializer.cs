using Godot;
using System;

public partial class Initializer : Node
{
	public override void _Ready()
	{	
		Engine.TimeScale = 3.5;
		Engine.PhysicsTicksPerSecond = 60;
		GD.Print("OK!");
	}
	
	public override void _Process(double delta)
	{
		
	}

	public Initializer()
	{
		
	}
}
