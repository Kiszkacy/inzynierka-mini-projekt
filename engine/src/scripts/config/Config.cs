
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Config : Singleton<Config>
{
    private string configPath = "./src/config.yaml";
    
    public ConfigData Data { get; }

    private Config()
    {
        this.Data = ConfigData.Load(this.configPath);
    }
}

public class ConfigData
{
    public PipeConfig Pipe { get; set; } = new();
    public RewardsConfig Rewards { get; set; } = new();
    public EngineConfig Engine { get; set; } = new();
    public GameConfig Game { get; set; } = new();
    
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