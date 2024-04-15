using Godot;
using System;

public partial class UserInput : Node
{
	private Paddle leftPaddle;
	private Paddle rightPaddle;
	
	public override void _Ready()
	{
		this.leftPaddle = GetParent().GetNode<Paddle>("LeftPaddle");
		this.rightPaddle = GetParent().GetNode<Paddle>("RightPaddle");
	}
	
	public override void _Process(double delta)
	{
		
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("left-up"))
		{
			this.leftPaddle.GoUp();
		}
		else if (@event.IsActionReleased("left-up"))
		{
			this.leftPaddle.StopUp();
		}
		else if (@event.IsActionPressed("left-down"))
		{
			this.leftPaddle.GoDown();
		}
		else if (@event.IsActionReleased("left-down"))
		{
			this.leftPaddle.StopDown();
		}
		
		else if (@event.IsActionReleased("right-up"))
		{
			this.rightPaddle.GoUp();
		}
		else if (@event.IsActionReleased("right-up"))
		{
			this.rightPaddle.StopUp();
		} 
		else if (@event.IsActionPressed("right-down"))
		{
			this.rightPaddle.GoDown();
		}
		else if (@event.IsActionReleased("right-down"))
		{
			this.rightPaddle.StopDown();
		}
	}

	public UserInput()
	{
		
	}
}
