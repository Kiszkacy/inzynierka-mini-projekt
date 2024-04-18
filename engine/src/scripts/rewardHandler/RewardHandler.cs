using Godot;
using System;

public partial class RewardHandler : Node, Observable
{
    private int rewardNow;
    private int rewardBefore;
    public int Reward {
        get {
            int reward = rewardNow - rewardBefore;
            rewardBefore = rewardNow;
            return reward;
        }
    }

	public override void _Ready()
	{

	}
	
	public override void _Process(double delta)
	{
		
	}


    public void Notify(Event @event)
    {   
        rewardBefore = rewardNow;

        if (@event.Code == "SIDE.RIGHT.BOUNCE")
        {
            rewardNow += 1;
        }
        else if (@event.Code == "SIDE.RIGHT.SCORE")
        {
            rewardNow += 20;
        }
        else if (@event.Code == "SIDE.LEFT.SCORE")
        {
            rewardNow -= 20;
        }
    }

    public RewardHandler()
	{
		
	}
}