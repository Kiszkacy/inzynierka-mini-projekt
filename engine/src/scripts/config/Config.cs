
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Config : Singleton<Config>
{
    private string configPath = "./src/config.yaml";
    
    public ConfigData Data { get; }
    
    public PipeConfig Pipe => this.Data.Pipe;
    public RewardsConfig Rewards => this.Data.Rewards;
    public EngineConfig Engine => this.Data.Engine;
    public GameConfig Game => this.Data.Game;
    public TestsConfig Tests => this.Data.Tests;

    private Config()
    {
        this.Data = ConfigData.Load(this.configPath);
    }
}

public class ConfigData
{
    public PipeConfig Pipe => new();
    public RewardsConfig Rewards => new();
    public EngineConfig Engine => new();
    public GameConfig Game => new();
    public TestsConfig Tests => new();
    
    public static ConfigData Load(string path)
    {
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        string yaml = File.ReadAllText(path);
        ConfigData config = deserializer.Deserialize<ConfigData>(yaml);
        return config;
    }
}

public class PipeConfig
{
    public string Name { get; set; }
    public int BufferSize { get; set; }
}

public class RewardsConfig
{
    public int BounceReward { get; set; }
    public int ScoreReward { get; set; }
    public int EnemyScorePenalty { get; set; }
    public int PaddlePositionReward { get; set; }
}

public class EngineConfig
{
    public float TimeScale { get; set; }
    public int TicksPerSecond { get; set; }
}

public class GameConfig
{
    public int? MaxScore { get; set; }
    public int BallPositionHistoryBufferSize { get; set; }
    public float BallStuckThreshold { get; set; }
    public float BallFlewThroughMovementSlowdown { get; set; }
}

public class TestsConfig
{
    public bool RunTests { get; set; }
    public bool RunTestsWhenOpenedViaCommandLine { get; set; }
    public bool RunSlowTests { get; set; }
    public bool PassUncertainTestsWhenFailed { get; set; }
    public bool PrintAdditionalLogs { get; set; }
}