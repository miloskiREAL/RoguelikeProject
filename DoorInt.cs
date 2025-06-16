using Godot;
using System;

public partial class DoorInt : TileMapLayer
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
			// Advance to the next floor and clear the current dungeon layout
			// so the next dungeon floor will be random
			GameManager.Instance.SaveData.Floor++;
			
			// Clear stored layout
			GameManager.Instance.SaveData.CurrentDungeonLayout = 0; 
			
			// Clear any battle return position since we're advancing floors
			GameManager.Instance.ClearBattlePosition();
			
			GetTree().ChangeSceneToFile("res://Scenes/Dungeon.tscn");
		}
	}
}
