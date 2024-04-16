using Godot;
using System;

public partial class Ball : CharacterBody2D, Observable
{
	[Export] 
	public double LaunchSpeed { get; set; } = 600.0; // in px/sec
	
	[Export]
	public double BounceAngleRandomness { get; set; } = 20.0; // range in degrees (from X/2, to X/2)
	
	[Export]
	public double ReverseBounceAngleChance { get; set; } = 25.0; // in percentages

	[Export] 
	public double BounceMaxAngle { get; set; } = 40.0; // in degrees (relative to the bottom of the screen)
	
	[Export] 
	public double BounceMinAngle { get; set; } = 10.0; // in degrees (relative to the bottom of the screen)
	
	[Export] 
	public double LaunchAngleRandomness { get; set; } = 50.0; // range in degrees (from X/2 - MinimalLaunchAngle, to X/2 + MinimalLaunchAngle)
	
	[Export] 
	public double MinimalLaunchAngle { get; set; } = 10.0; // range in degrees (from X/2, to X/2)
	
	[Export] 
	public double SpeedIncreaseOnBounce { get; set; } = 20.0; // in px/sec
	
	[Export] 
	public double MaxSpeed { get; set; } = 1000.0; // in px/sec
	
	public override void _Ready()
	{
		EventManager.Get().Subscribe(this);
	}

	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		this.MovementProcess(delta);
	}
	
	private void MovementProcess(double delta)
	{
		KinematicCollision2D collision = this.MoveAndCollide(this.Velocity * (float)delta);
		if (collision == null) return;
		
		Vector2 normal = collision.GetNormal();
		this.Velocity = this.Velocity.Bounce(normal);
		this.ApplyBounceAngle();
		this.AdjustSpeed();
		this.ConditionallyReverseBounceAngle(normal);
	}
	
	private void ApplyBounceAngle()
	{
		double bounceAngleInDegrees = this.ClampBounceAngle();
		double angleNoiseInDegrees = new RandomNumberGenerator().RandfRange(-(float)this.BounceAngleRandomness/2.0f, (float)this.BounceAngleRandomness/2.0f);
		
		double finalAngleInDegrees = bounceAngleInDegrees + angleNoiseInDegrees;
		double finalAngleInRadians = Mathf.DegToRad(finalAngleInDegrees);
		Vector2 finalBounceDirection = new Vector2(Mathf.Cos((float)finalAngleInRadians), Mathf.Sin((float)finalAngleInRadians));
		this.Velocity = finalBounceDirection * this.Velocity.Length();
	}
	
	private double ClampBounceAngle()
	{
		Vector2 bounceDirection = this.Velocity.Normalized();
		double bounceAngleInDegrees = Mathf.RadToDeg(Mathf.Atan2(bounceDirection.Y, bounceDirection.X));
		double bounceRelativeAngleInDegrees = Mathf.Abs(bounceAngleInDegrees) > 90.0f ? Math.Abs(Math.Abs(bounceAngleInDegrees) - 180.0f) : Math.Abs(bounceAngleInDegrees);
    
		if (bounceRelativeAngleInDegrees > this.BounceMaxAngle)
		{
			bool isLeftSideBounce = Mathf.Abs(bounceAngleInDegrees) > 90.0f;
			if (isLeftSideBounce)
			{
				bounceAngleInDegrees = (180.0f - this.BounceMaxAngle + this.BounceAngleRandomness/2.0f) * Mathf.Sign(bounceAngleInDegrees);
			}
			else
			{
				bounceAngleInDegrees = (this.BounceMaxAngle - this.BounceAngleRandomness/2.0f) * Mathf.Sign(bounceAngleInDegrees);
			}
		} 
		else if (bounceRelativeAngleInDegrees < this.BounceMinAngle)
		{
			bool isLeftSideBounce = Mathf.Abs(bounceAngleInDegrees) > 90.0f;
			if (isLeftSideBounce)
			{
				bounceAngleInDegrees = (180.0f + this.BounceMinAngle - this.BounceAngleRandomness/2.0f) * Mathf.Sign(bounceAngleInDegrees);
			}
			else
			{
				bounceAngleInDegrees = (this.BounceMinAngle + this.BounceAngleRandomness/2.0f) * Mathf.Sign(bounceAngleInDegrees);
			}
		}

		return bounceAngleInDegrees;
	}
	
	private void AdjustSpeed()
	{
		double newSpeed = Mathf.Clamp(this.Velocity.Length() + this.SpeedIncreaseOnBounce, 0.0f, this.MaxSpeed);
		this.Velocity = this.Velocity.Normalized() * (float)newSpeed;
	}
	
	private void ConditionallyReverseBounceAngle(Vector2 normal)
	{
		double bounceAngleRelativeToBottomInDegrees = Mathf.RadToDeg(Vector2.Up.AngleTo(normal));
		double comparisonThreshold = 0.01f;
		bool isPaddleBounce = Math.Abs(bounceAngleRelativeToBottomInDegrees - 90) < comparisonThreshold || Math.Abs(bounceAngleRelativeToBottomInDegrees + 90) < comparisonThreshold;
		if (isPaddleBounce && new RandomNumberGenerator().Randf() <= this.ReverseBounceAngleChance / 100.0f)
		{
			this.Velocity = new Vector2(this.Velocity.X, -this.Velocity.Y);
		}
	}

	private void LaunchItself(Side side = Side.Left)
	{
		double launchAngleInDegrees = new RandomNumberGenerator().RandfRange(-(float)this.LaunchAngleRandomness/2.0f, (float)this.LaunchAngleRandomness/2.0f);
		launchAngleInDegrees += Mathf.Sign(launchAngleInDegrees) * this.MinimalLaunchAngle;
		double launchAngleInRadians = Mathf.DegToRad(side == Side.Left ? 180.0f + launchAngleInDegrees : launchAngleInDegrees);
		Vector2 launchDirection = new Vector2(Mathf.Cos((float)launchAngleInRadians), Mathf.Sin((float)launchAngleInRadians));
		this.Launch(launchDirection, this.LaunchSpeed);
	}

	public void Launch(Vector2 direction, double speed)
	{
		this.Velocity = direction.Normalized() * (float)speed;
	}
	
	public void Notify(Event @event)
	{
		if (@event.Code == "RESET")
		{
			this.GlobalPosition = GetViewportRect().Size / 2.0f;
			this.LaunchItself();
		}
		else if (@event.Code == "BALL.LAUNCH.LEFT")
		{
			this.GlobalPosition = GetViewportRect().Size / 2.0f;
			this.LaunchItself(Side.Left);
		}
		else if (@event.Code == "BALL.LAUNCH.RIGHT")
		{
			this.GlobalPosition = GetViewportRect().Size / 2.0f;
			this.LaunchItself(Side.Right);
		}
	}

	public Ball()
	{
		
	}
}
