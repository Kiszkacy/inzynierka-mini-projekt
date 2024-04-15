using Godot;
using System;

public partial class Paddle : Node2D
{
	private double speed = 5.0;
	private bool movingUp;
	private bool movingDown;
	
	[Export]
	public double Speed
	{
		get => this.speed;
		set => this.speed = value;
	}
	public override void _Ready()
	{
		
	}

	private void MovementProcess(double delta)
	{
		Vector2 direction = Vector2.Zero;
		direction += this.movingUp ? Vector2.Up : Vector2.Zero; 
		direction += this.movingDown ? Vector2.Down : Vector2.Zero; 
		
		this.GlobalPosition += direction * (float)this.speed * (float)delta;
	}
	
	public override void _Process(double delta)
	{
		this.MovementProcess(delta);
	}

	public void GoUp()
	{
		this.movingUp = true;
	}
	
	public void StopUp()
	{
		this.movingUp = false;
	}
	
	public void GoDown()
	{
		this.movingDown = true;
	}
	
	public void StopDown()
	{
		this.movingDown = false;
	}

	public Paddle()
	{
		
	}
}
