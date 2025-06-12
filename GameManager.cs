using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	public SaveData SaveData { get; private set; }
	
	// Track which save slot we're currently using
	private int currentSaveSlot = -1;
	
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
		GD.Print($"Save data loaded into GameManager from slot {saveSlot}.");
	}

	public void ResetGame()
	{
		SaveData = new SaveData();
		currentSaveSlot = -1;
		GD.Print("GameManager reset with new SaveData.");
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
			string json = Json.Stringify(SaveData.ToDictionary(), "\t");

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
}
