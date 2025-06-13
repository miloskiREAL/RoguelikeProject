using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public int Speed = 100;
	[Export] public float EncounterRate = 0.008f; // Chance per step (0.8% per step)
	[Export] public int MinStepsBetweenEncounters = 10; // Minimum steps before next encounter can occur
	
	private AnimatedSprite2D animatedSprite;
	private int stepsSinceLastEncounter = 0;
	private bool wasMovingLastFrame = false;
	private Vector2 lastPosition;
	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedPlayer");
		lastPosition = GlobalPosition;
		
		// Load battle return position if returning from battle
		if (GameManager.Instance.HasBattleReturnPosition)
		{
			GlobalPosition = GameManager.Instance.GetAndClearBattleReturnPosition();
			GD.Print($"Returned from battle, restored position: {GlobalPosition}");
		}
		// Load regular saved position if it exists (for normal game loads)
		else if (GameManager.Instance.SaveData.HasSavedPosition)
		{
			GlobalPosition = GameManager.Instance.SaveData.SavedPosition;
			GameManager.Instance.SaveData.HasSavedPosition = false;
			GD.Print($"Loaded saved position: {GlobalPosition}");
		}
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
		
		// Check for movement steps for random encounters
		CheckForRandomEncounter(velocity);
	}
	
	private void CheckForRandomEncounter(Vector2 velocity)
	{
		bool isMovingNow = velocity.Length() > 0;
		
		// Check if we've moved a significant distance (one "step")
		if (isMovingNow && GlobalPosition.DistanceTo(lastPosition) > 16) // Adjust step size as needed
		{
			stepsSinceLastEncounter++;
			lastPosition = GlobalPosition;
			
			// Only check for encounters after minimum steps and if we're in a dungeon floor
			if (stepsSinceLastEncounter >= MinStepsBetweenEncounters && CanHaveRandomEncounter())
			{
				if (GD.Randf() < EncounterRate)
				{
					TriggerRandomEncounter();
				}
			}
		}
	}
	
	private bool CanHaveRandomEncounter()
	{
		// Only allow encounters on dungeon floors, not on start, boss, rest, or final boss floors
		int floor = GameManager.Instance.SaveData.Floor;
		var floorType = DungeonManager.FloorHelper.GetFloorType(floor);
		
		return floorType == DungeonManager.FloorType.Dungeon;
	}
	
	private void TriggerRandomEncounter()
	{
		// Save current position for battle return (separate from main game save)
		GameManager.Instance.QuickSaveForBattle(GlobalPosition);
		
		int block = DungeonManager.FloorHelper.GetBlock(GameManager.Instance.SaveData.Floor);
		int version = GD.RandRange(1, 3); 
		
		// Reset encounter counter
		stepsSinceLastEncounter = 0;
		
		// Load battle scene
		string battleScene = $"res://Scenes/BattleSceneBlock{block}Version{version}.tscn";
		GD.Print($"Random encounter! Loading scene: {battleScene}");
		GetTree().ChangeSceneToFile(battleScene);
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
