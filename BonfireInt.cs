using Godot;
using System;

public partial class BonfireInt : StaticBody2D
{
	private bool inRange = false;
	
	public override void _Ready()
	{
		GetNode<Area2D>("InteractArea").BodyEntered += OnPlayerEnter;
		GetNode<Area2D>("InteractArea").BodyExited += OnPlayerExit;
	}
	
	private void OnPlayerEnter(Node body)
	{
		if (body is Player)
		{
			inRange = true;
			
			GetNode<AnimatedSprite2D>("InteractArea/Exclamation").Visible = true;
		}
	}
	
	private void OnPlayerExit(Node body)
	{
		if (body is Player)
		{
			inRange = false;
			
			GetNode<AnimatedSprite2D>("InteractArea/Exclamation").Visible = false;
		}
	}
	
	public override void _Process(double delta)
	{
		if (inRange && Input.IsActionJustPressed("interact"))
		{
			
		}
	}
}
