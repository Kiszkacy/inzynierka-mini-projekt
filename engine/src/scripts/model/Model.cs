
using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;

public class Model : Agent, Observable
{   
	private bool hasGameEndedThisFrame;

	public Action GetAction(Side side, double leftPaddlePosition, double rightPaddlePosition, Vector2 ballPosition, Vector2 ballVelocity)
	{   
		double[] state = {leftPaddlePosition, rightPaddlePosition, ballPosition.X, ballPosition.Y, ballVelocity.X, ballVelocity.Y};
		
		Dictionary<string, object> dict = new Dictionary<string, object>{
			["state"] = state,
			["reward"] = RewardHandler.Get().Reward,
			["is_done"] = hasGameEndedThisFrame,
		};
		RewardHandler.Get().ResetReward();
		
		byte[] rawData = JsonSerializer.Serialize(dict).ToUtf8Buffer(); 
		PipeHandler.Get().Send(rawData);
		
		if (this.hasGameEndedThisFrame) this.hasGameEndedThisFrame = false;

		byte[] response = PipeHandler.Get().Receive();
		int responseCode = BitConverter.ToInt16(response);
		if (responseCode == -1)
		{
			EventManager.Get().RegisterEvent(new Event("RESET"));
			return Action.None;
		}
		return (Action)responseCode;
	}

	public void Notify(Event @event)
	{
		if (@event.Code == "SIDE.LEFT.WON" || @event.Code == "SIDE.RIGHT.WON")
		{
			hasGameEndedThisFrame = true;
		}
	}

	public Model()
	{
		EventManager.Get().Subscribe(this);
	}
}
