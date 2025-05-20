using Godot;
using System;

public partial class InteractArea : StaticBody2D
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
			//HUD.Instance.ShowPrompt("Press [E] to trade");
			GetNode<AnimatedSprite2D>("InteractArea/Exclamation").Visible = true;
		}
	}
	private void OnPlayerExit(Node body)
	{
		if (body is Player)
		{
			inRange = false;
			//HUD.Instance.ShowPrompt("Press [E] to trade");
			GetNode<AnimatedSprite2D>("InteractArea/Exclamation").Visible = false;
		}
	}
}
