
public class RewardHandler : Singleton<RewardHandler>, Observable
{
    private int rewardNow;
    private int rewardBefore;

    private const int BounceReward = 1;
    private const int ScoreReward = 20;
    private const int EnemyScorePenalty = -20;
    
    public int Reward {
        get {
            int reward = this.rewardNow - this.rewardBefore;
            this.rewardBefore = this.rewardNow;
            return reward;
        }
    }

    public void Notify(Event @event)
    {   
        this.rewardBefore = this.rewardNow;

        if (@event.Code == "SIDE.RIGHT.BOUNCE")
        {
            this.rewardNow += BounceReward;
        }
        else if (@event.Code == "SIDE.RIGHT.SCORE")
        {
            this.rewardNow += ScoreReward;
        }
        else if (@event.Code == "SIDE.LEFT.SCORE")
        {
            this.rewardNow -= EnemyScorePenalty;
        }
    }

    private RewardHandler()
	{
		
	}
}