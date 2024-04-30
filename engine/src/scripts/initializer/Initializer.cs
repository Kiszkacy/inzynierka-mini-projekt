using Godot;
using System;

public partial class Initializer : Node
{
	public override void _Ready()
	{
		this.LoadSingletons();
		this.SetupEngineSettings();
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

	private void LoadSingletons() // this method loads singletons that are required to be loaded in a specific order
	{
		Config.Get();
		EventManager.Get();
	}

	private void SetupEngineSettings()
	{
		Engine.TimeScale = 3.0;
		Engine.PhysicsTicksPerSecond = 60;
	}

	public Initializer()
	{
		
	}
}
