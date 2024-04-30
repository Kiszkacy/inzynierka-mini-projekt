using Godot;

public partial class Initializer : Node
{
	public override void _Ready()
	{
		GD.Print("Initializer: Loading scene.");
		GD.Print("Initializer: Loading singletons.");
		this.LoadSingletons();
		GD.Print("Initializer: Setting up engine settings.");
		this.SetupEngineSettings();
		GD.Print("Initializer: Initial load complete.");
	}

	private void LoadSingletons() // this method loads singletons that are required to be loaded in a specific order
	{
		GD.Print("Initializer: Loading config.");
		this.LoadConfig();
		EventManager.Get();
	}

	private void LoadConfig()
	{
		Config.Get();
		CommandLineReader.ParseCustomArguments();
	}
	
	private void SetupEngineSettings()
	{
		Engine.TimeScale = Config.Get().Data.Engine.TimeScale;
		Engine.PhysicsTicksPerSecond = Config.Get().Data.Engine.TicksPerSecond;
	}

	public Initializer()
	{
		
	}
}
