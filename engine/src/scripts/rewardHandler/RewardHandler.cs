
public class RewardHandler : Singleton<RewardHandler>, Observable
{
    private const int BounceReward = 1;
    private const int ScoreReward = 20;
    private const int EnemyScorePenalty = -20;
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