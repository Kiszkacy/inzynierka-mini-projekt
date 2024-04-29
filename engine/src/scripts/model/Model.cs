
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Godot;

public class Model : Agent, Observable
{   
    private SocketHandler socketHandler;
    private RewardHandler rewardHandler;
    private bool gameEnded;

	public Action GetAction(Side side, double leftPaddlePosition, double rightPaddlePosition, Vector2 ballPosition, Vector2 ballVelocity)
	{   
		double[] state = {leftPaddlePosition, rightPaddlePosition, ballPosition.X, ballPosition.Y, ballVelocity.X, ballVelocity.Y};

		Dictionary<string, object> dict = new Dictionary<string, object>{
			["state"] = state,
			["reward"] = rewardHandler.Reward,
			["is_done"] = gameEnded,
		};
		
		byte[] rawData = JsonSerializer.Serialize(dict).ToUtf8Buffer(); 
		socketHandler.Send(rawData, rawData.Length);
		byte[] response = socketHandler.Receive();
		gameEnded = false;
		int value = BitConverter.ToInt16(response);
		if (value == -1){
			EventManager.Get().RegisterEvent(new Event("RESET"));
			return Action.None;
		}
		return (Action)value;
	}

	public void Notify(Event @event)
	{
		if (@event.Code == "SIDE.LEFT.WON" || @event.Code == "SIDE.RIGHT.WON")
		{
			gameEnded = true;
		}
	}

	public Model(SocketHandler socketHandler, RewardHandler rewardHandler)
	{
		this.socketHandler = socketHandler;
		this.rewardHandler = rewardHandler;
	}
}
