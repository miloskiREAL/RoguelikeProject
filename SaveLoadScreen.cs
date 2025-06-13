using Godot;
using System;
using System.Collections.Generic;

public partial class SaveLoadScreen : Control
{
	private string[] savePaths = {
		"user://save1.json",
		"user://save2.json",
		"user://save3.json"
	};
	private Button[] loadButtons = new Button[3];
	private Button[] deleteButtons = new Button[3];
	private Label[] statusLabels = new Label[3];
	[Export] public PackedScene startScreen;

	public override void _Ready()
	{
		for (int i = 0; i < 3; i++)
		{
			int index = i; 
			string path = savePaths[i];
			statusLabels[i] = GetNode<Label>($"CenterContainer2/VBoxContainer/StatusLabel{i + 1}");
			loadButtons[i] = GetNode<Button>($"CenterContainer/VBoxContainer/SaveSlot{i + 1}");
			deleteButtons[i] = GetNode<Button>($"VBoxContainer/DeleteButton{i + 1}");
			GD.Print(statusLabels[i]);

			// Check if save exists
			bool exists = FileAccess.FileExists(path);
			GD.Print($"Checking for: user://save{i + 1}.json");

			if (exists)
			{
				var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
				var json = file.GetAsText();
				file.Close();

				var godotData = Json.ParseString(json).AsGodotDictionary();
				var data = ConvertGodotDictToStringVariantDict(godotData);
				var saveData = new SaveData(data);

				statusLabels[i].Text = $"Save {i + 1} - Level {saveData.PartyLevel}";
				loadButtons[i].Disabled = false;
				deleteButtons[i].Disabled = false;
			}
			else
			{
				statusLabels[i].Text = $"Save {i + 1} - Empty";
				loadButtons[i].Disabled = false;
				deleteButtons[i].Disabled = true;
			}

			// Connect buttons
			loadButtons[i].Pressed += () => LoadOrCreateGame(index);
			deleteButtons[i].Pressed += () => DeleteSave(index);
		}

		// Connect return to menu
		var backButton = GetNode<Button>("BackButton");
		backButton.Pressed += () =>
		{
			GetTree().ChangeSceneToFile("res://Scenes/StartScreen.tscn");
		};
	}

	public void LoadOrCreateGame(int slot)
	{
		string path = savePaths[slot];
		SaveData saveData;

		if (FileAccess.FileExists(path))
		{
			var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			var json = file.GetAsText();
			file.Close();

			var godotData = Json.ParseString(json).AsGodotDictionary();
			var data = ConvertGodotDictToStringVariantDict(godotData);
			saveData = new SaveData(data);
		}
		else
		{
			saveData = new SaveData(); // Create new save with default values
			CreateNewSave(slot, saveData);
		}

		GameManager.Instance.LoadFromSaveData(saveData, slot);
		GetTree().ChangeSceneToFile("res://Scenes/Dungeon.tscn");
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

	public void CreateNewSave(int slot, SaveData saveData)
	{
		string path = savePaths[slot];
		var saveDict = saveData.ToDictionary();
		var godotDict = ConvertStringVariantDictToGodotDict(saveDict);
		string json = Json.Stringify(godotDict, "\t");

		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		file.StoreString(json);
		file.Close();
	}

	// Convert Godot.Collections.Dictionary to Dictionary<string, Variant>
	private Dictionary<string, Variant> ConvertGodotDictToStringVariantDict(Godot.Collections.Dictionary godotDict)
	{
		var result = new Dictionary<string, Variant>();
		foreach (var key in godotDict.Keys)
		{
			if (key.VariantType == Variant.Type.String)
			{
				result[key.AsString()] = godotDict[key];
			}
		}
		return result;
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
