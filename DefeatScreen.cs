using Godot;
using System;

public partial class DefeatScreen : Control
{
	[Export] private NodePath returnToStartButtonPath;
	[Export] private NodePath quitGameButtonPath;
	[Export] private NodePath defeatMessagePath;

	private Button returnToStartButton;
	private Button quitGameButton;
	private Label defeatMessage;

	public override void _Ready()
	{
		// Get node references
		returnToStartButton = GetNode<Button>(returnToStartButtonPath);
		quitGameButton = GetNode<Button>(quitGameButtonPath);
		defeatMessage = GetNode<Label>(defeatMessagePath);

		// Connect button signals
		returnToStartButton.Pressed += OnReturnToStartPressed;
		quitGameButton.Pressed += OnQuitGamePressed;

		// Set up the defeat message
		if (defeatMessage != null)
		{
			defeatMessage.Text = "DEFEAT\n\nYour party has been defeated in the dungeon.";
		}

		// Make sure the scene is visible and focused
		Show();
		returnToStartButton?.GrabFocus();
	}

	private void OnReturnToStartPressed()
	{
		GD.Print("Return to Start selected - resetting progress...");
		
		// Get the GameManager instance
		var gameManager = GameManager.Instance;
		if (gameManager?.SaveData != null)
		{
			// Reset inventory and wildcards while preserving level and XP
			int preservedLevel = gameManager.SaveData.PartyLevel;
			int preservedXP = gameManager.SaveData.XP;
			
			// Clear inventory, wildcards, gold, and reset floor
			gameManager.SaveData.Inventory.Clear();
			gameManager.SaveData.WildCards.Clear();
			gameManager.SaveData.Gold = 0;
			gameManager.SaveData.Floor = 0;
			
			// Clear boss trigger data
			gameManager.SaveData.TriggeredBossWires.Clear();
			
			// Reset dungeon layout as well
			gameManager.SaveData.CurrentDungeonLayout = 0; 
			
			// Clear opened chests data
			gameManager.SaveData.OpenedChests.Clear();
			gameManager.SaveData.OpenedChestsCurrentFloor.Clear();
			
			// Restore level and XP
			gameManager.SaveData.PartyLevel = preservedLevel;
			gameManager.SaveData.XP = preservedXP;
			
			// Clear any saved position data
			gameManager.SaveData.SavedPosition = Vector2.Zero;
			gameManager.SaveData.HasSavedPosition = false;
			gameManager.ClearBattlePosition();
			
			GD.Print($"Progress reset - Level {preservedLevel} preserved, inventory/wildcards/gold/boss triggers cleared, floor reset to 0");
			
			// Try to save the reset state if we have a current save slot
			if (gameManager.GetCurrentSaveSlot() >= 0)
			{
				gameManager.SaveGame();
				GD.Print("Reset state saved to current save slot");
			}
		}
		else
		{
			GD.PrintErr("GameManager or SaveData not found - creating new save data");
			gameManager?.ResetGame();
		}
		
		// Return to the dungeon scene
		GetTree().ChangeSceneToFile("res://Scenes/Dungeon.tscn");
	}

	private void OnQuitGamePressed()
	{
		GD.Print("Quit Game selected - resetting progress and exiting...");
		
		// Get the GameManager instance
		var gameManager = GameManager.Instance;
		if (gameManager?.SaveData != null)
		{
			// Reset everything except level and XP 
			int preservedLevel = gameManager.SaveData.PartyLevel;
			int preservedXP = gameManager.SaveData.XP;
			
			// Clear inventory, wildcards, gold, and reset floor
			gameManager.SaveData.Inventory.Clear();
			gameManager.SaveData.WildCards.Clear();
			gameManager.SaveData.Gold = 0;
			gameManager.SaveData.Floor = 0;
			
			// Clear boss trigger data
			gameManager.SaveData.TriggeredBossWires.Clear();
			
			// Reset dungeon layout as well
			gameManager.SaveData.CurrentDungeonLayout = 0; 
			
			// Clear opened chests data
			gameManager.SaveData.OpenedChestsCurrentFloor.Clear();
			gameManager.SaveData.OpenedChests.Clear();
			
			// Restore level and XP
			gameManager.SaveData.PartyLevel = preservedLevel;
			gameManager.SaveData.XP = preservedXP;
			
			// Clear any saved position data
			gameManager.SaveData.SavedPosition = Vector2.Zero;
			gameManager.SaveData.HasSavedPosition = false;
			gameManager.ClearBattlePosition();
			
			GD.Print($"Progress reset - Level {preservedLevel} preserved, inventory/wildcards/gold/boss triggers cleared, floor reset to 0");
			
			// Try to save the reset state
			if (gameManager.GetCurrentSaveSlot() >= 0)
			{
				gameManager.SaveGame();
				GD.Print("Reset state saved to current save slot");
			}
		}
		else
		{
			GD.PrintErr("GameManager or SaveData not found during quit");
		}
		
		// Close the game
		GetTree().Quit();
	}

	// Handle escape key to focus on return button
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode == Key.Escape)
			{
				returnToStartButton?.GrabFocus();
			}
		}
	}
}
