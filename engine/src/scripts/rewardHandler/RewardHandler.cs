
using Godot;

public class RewardHandler : Singleton<RewardHandler>, Observable
{
    private readonly int BounceReward = Config.Get().Data.Rewards.BounceReward;
    private readonly int ScoreReward = Config.Get().Data.Rewards.ScoreReward;
    private readonly int EnemyScorePenalty = Config.Get().Data.Rewards.EnemyScorePenalty;
    private readonly int PaddlePositionReward = Config.Get().Data.Rewards.PaddlePositionReward;
    private int reward;
    public int Reward => this.reward;

    public void Notify(Event @event)
    {
        if (@event.Code == "SIDE.RIGHT.BOUNCE")
        {
            this.reward += BounceReward;
        }
        else if (@event.Code == "SIDE.RIGHT.SCORE")
        {
            this.reward += ScoreReward;
        }
        else if (@event.Code == "SIDE.LEFT.SCORE")
        {
            this.reward += EnemyScorePenalty;
        }
    }

    public void ResetReward()
    {
        this.reward = 0;
    }

    private RewardHandler()
	{
		EventManager.Get().Subscribe(this);
	}
}