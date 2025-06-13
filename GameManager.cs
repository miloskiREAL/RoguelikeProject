using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	public SaveData SaveData { get; private set; }
	
	// Track which save slot we're currently using
	private int currentSaveSlot = -1;
	
	// Battle-specific temporary position storage (separate from main save data)
	public Vector2 BattleReturnPosition { get; set; } = Vector2.Zero;
	public bool HasBattleReturnPosition { get; set; } = false;
	
	private string[] savePaths = {
		"user://save1.json",
		"user://save2.json",
		"user://save3.json"
	};

	public override void _Ready()
	{
		if (Instance != null && Instance != this)
		{
			GD.PrintErr("Duplicate GameManager found. Deleting this one.");
			QueueFree();
			return;
		}

		Instance = this;
		GD.Print("GameManager initialized.");
		SetProcess(false);
		this.Owner = null;
	}

	public void LoadFromSaveData(SaveData data, int saveSlot = -1)
	{
		SaveData = data;
		currentSaveSlot = saveSlot;
		// Clear any battle position data when loading a save
		ClearBattlePosition();
		GD.Print($"Save data loaded into GameManager from slot {saveSlot}.");
	}

	public void ResetGame()
	{
		SaveData = new SaveData();
		currentSaveSlot = -1;
		ClearBattlePosition();
		GD.Print("GameManager reset with new SaveData.");
	}

	// Set battle return position (used when entering battle)
	public void SetBattleReturnPosition(Vector2 position)
	{
		BattleReturnPosition = position;
		HasBattleReturnPosition = true;
		GD.Print($"Battle return position set to: {position}");
	}

	// Get and clear battle return position (used when returning from battle)
	public Vector2 GetAndClearBattleReturnPosition()
	{
		Vector2 position = BattleReturnPosition;
		ClearBattlePosition();
		return position;
	}

	// Clear battle position data
	public void ClearBattlePosition()
	{
		BattleReturnPosition = Vector2.Zero;
		HasBattleReturnPosition = false;
	}

	public bool SaveGame(int saveSlot = -1)
	{
		if (SaveData == null)
		{
			GD.PrintErr("Cannot save: No SaveData available!");
			return false;
		}

		int slotToUse = saveSlot >= 0 ? saveSlot : currentSaveSlot;
		
		if (slotToUse < 0 || slotToUse >= savePaths.Length)
		{
			GD.PrintErr($"Invalid save slot: {slotToUse}. Must be 0-2.");
			return false;
		}

		try
		{
			string path = savePaths[slotToUse];
			var saveDict = SaveData.ToDictionary();
			var godotDict = ConvertStringVariantDictToGodotDict(saveDict);
			string json = Json.Stringify(godotDict, "\t");

			using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
			if (file == null)
			{
				GD.PrintErr($"Failed to open file for writing: {path}");
				return false;
			}

			file.StoreString(json);
			file.Close();

			if (saveSlot >= 0)
			{
				currentSaveSlot = saveSlot;
			}

			GD.Print($"Game saved successfully to slot {slotToUse + 1}");
			return true;
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error saving game: {e.Message}");
			return false;
		}
	}

	// Quick save for battle system - this does NOT save the main game state
	// It only preserves the current position for battle return
	public bool QuickSaveForBattle(Vector2 playerPosition)
	{
		SetBattleReturnPosition(playerPosition);
		GD.Print("Battle position saved for return.");
		return true;
	}

	// Regular quick save for main game progression
	public bool QuickSave()
	{
		if (currentSaveSlot < 0)
		{
			GD.PrintErr("Cannot quick save: No current save slot set!");
			return false;
		}
		
		return SaveGame(currentSaveSlot);
	}

	public int GetCurrentSaveSlot()
	{
		return currentSaveSlot;
	}

	public bool SaveExists(int saveSlot)
	{
		if (saveSlot < 0 || saveSlot >= savePaths.Length)
			return false;
			
		return FileAccess.FileExists(savePaths[saveSlot]);
	}

	// Convert Dictionary<string, Variant> to Godot.Collections.Dictionary
	private Godot.Collections.Dictionary ConvertStringVariantDictToGodotDict(Dictionary<string, Variant> dict)
	{
		var godotDict = new Godot.Collections.Dictionary();
		foreach (var pair in dict)
		{
			godotDict[pair.Key] = pair.Value;
		}
		return godotDict;
	}
}
