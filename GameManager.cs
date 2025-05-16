using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	public SaveData SaveData { get; private set; }

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

	public void LoadFromSaveData(SaveData data)
	{
		SaveData = data;
		GD.Print("Save data loaded into GameManager.");
	}

	public void ResetGame()
	{
		SaveData = new SaveData(); // Reset to defaults
		GD.Print("GameManager reset with new SaveData.");
	}

	// Optional: Add save/load helpers here if needed later
}
