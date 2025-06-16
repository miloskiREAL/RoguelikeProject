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
		if (body is Player player)
		{
			inRange = true;
			
			// Show interaction indicator
			GetNode<AnimatedSprite2D>("InteractArea/Exclamation").Visible = true;
		}
	}
	
	private void OnPlayerExit(Node body)
	{
		if (body is Player)
		{
			inRange = false;
			
			// Hide interaction indicator
			GetNode<AnimatedSprite2D>("InteractArea/Exclamation").Visible = false;
		}
	}
	
	public override void _Process(double delta)
	{
		if (inRange && Input.IsActionJustPressed("interact"))
		{
			// Save current position before entering shop
			var player = GetTree().GetFirstNodeInGroup("player") as Player;
			if (player != null)
			{
				GameManager.Instance.SaveData.SavedPosition = player.GlobalPosition;
				GameManager.Instance.SaveData.HasSavedPosition = true;
				GD.Print($"Saved position before entering shop: {player.GlobalPosition}");
			}
			
			// Save the game state
			GameManager.Instance.SaveGame();
			
			// Enter the shop
			GetTree().ChangeSceneToFile("res://Scenes/Shop.tscn");
		}
	}
}
