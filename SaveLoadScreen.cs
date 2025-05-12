using Godot;
using System;
using System.IO;

public partial class SaveLoadScreen : Control
{
	private string[] savePaths = {
		"user://save_slot_1.json",
		"user://save_slot_2.json",
		"user://save_slot_3.json"
	};

	private Button[] loadButtons = new Button[3];
	private Button[] deleteButtons = new Button[3];
	private Label[] statusLabels = new Label[3];

	[Export] public PackedScene mainMenuScene;

	public override void _Ready()
	{
		for (int i = 0; i < 3; i++)
		{
			int index = i;
			string path = savePaths[i];

			// Grab UI nodes dynamically
			statusLabels[i] = GetNode<Label>($"SaveSlot{index + 1}/StatusLabel");
			loadButtons[i] = GetNode<Button>($"SaveSlot{index + 1}/LoadButton");
			deleteButtons[i] = GetNode<Button>($"SaveSlot{index + 1}/DeleteButton");

			// Check if save exists
			bool exists = FileAccess.FileExists(path);

			if (exists)
			{
				var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
				var json = file.GetAsText();
				file.Close();

				var data = Json.ParseString(json).AsGodotDictionary();
				var saveData = new SaveData(data);

				statusLabels[i].Text = $"Save {index + 1} - Level {saveData.PartyLevel}";
				loadButtons[i].Disabled = false;
				deleteButtons[i].Disabled = false;
			}
			else
			{
				statusLabels[i].Text = $"Save {index + 1} - Empty";
				loadButtons[i].Disabled = true;
				deleteButtons[i].Disabled = true;
			}

			// Connect buttons
			loadButtons[i].Pressed += () => LoadGame(index);
			deleteButtons[i].Pressed += () => DeleteSave(index);
		}

		// Connect return button
		var backButton = GetNode<Button>("BackButton");
		backButton.Pressed += () =>
		{
			if (mainMenuScene != null)
				GetTree().ChangeSceneToPacked(mainMenuScene);
		};
	}

	public void LoadGame(int slot)
	{
		string path = savePaths[slot];

		if (FileAccess.FileExists(path))
		{
			var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			var json = file.GetAsText();
			file.Close();

			var data = Json.ParseString(json).AsGodotDictionary();
			var saveData = new SaveData(data);

			// Store in global singleton or GameManager
			GameManager.Instance.LoadFromSaveData(saveData);

			// Change to game scene
			GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
		}
	}

	public void DeleteSave(int slot)
	{
		string path = savePaths[slot];

		if (FileAccess.FileExists(path))
		{
			DirAccess.RemoveAbsolute(path);
			GD.Print($"Deleted save slot {slot + 1}");
			GetTree().ReloadCurrentScene();
		}
	}

	// Optional helper to create a new save file
	public void CreateNewSave(int slot, SaveData saveData)
	{
		string path = savePaths[slot];
		string json = Json.Stringify(saveData.ToDictionary(), "\t");

		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		file.StoreString(json);
		file.Close();
	}
}
