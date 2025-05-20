using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public int Speed = 150;
	private AnimatedSprite2D animatedSprite;
	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedPlayer");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (Input.IsActionPressed("ui_right"))
		{
			velocity.X += 1;
		}
			
		if (Input.IsActionPressed("ui_left"))
		{
			velocity.X -= 1;
		}
			
		if (Input.IsActionPressed("ui_down"))
		{
			velocity.Y += 1;
		}
		  
		if (Input.IsActionPressed("ui_up"))
		{
			velocity.Y -= 1;
		}
			
		velocity = velocity.Normalized() * Speed;
		Velocity = velocity;
		MoveAndSlide();
		UpdateAnimation(velocity);

	}
	
	private void UpdateAnimation(Vector2 velocity)
	{
		if (velocity == Vector2.Zero)
		{
			animatedSprite.Play("Idle"); // Or keep track of last direction
			return;
		}
		
		if (Mathf.Abs(velocity.X) > Mathf.Abs(velocity.Y))
		{
			if (velocity.X > 0)
			{
				animatedSprite.Play("walk right");
			}
			else
			{
				animatedSprite.Play("walk left");
			}
				
		}
		else
		{
			if (velocity.Y > 0)
			{
				animatedSprite.Play("walk down");
			}
				
			else 
			{
				animatedSprite.Play("walk up");
			}
				
		}
	}
}
