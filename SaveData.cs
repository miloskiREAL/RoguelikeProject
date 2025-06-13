using Godot;
using System;
using System.Collections.Generic;

public partial class SaveData : RefCounted
{
	// Basic stats
	public int PartyLevel { get; set; } = 1;
	public int XP { get; set; } = 0;
	public int Floor { get; set; } = 0;
	public int Gold { get; set; } = 0;
	public Dictionary<string, int> Inventory { get; set; } = new Dictionary<string, int>();
	public Dictionary<string, int> WildCards { get; set; } = new Dictionary<string, int>();
	
	// Position data - NOTE: This is now only for regular saves, not battle saves
	public Vector2 SavedPosition { get; set; } = Vector2.Zero;
	public bool HasSavedPosition { get; set; } = false;

	// Default constructor
	public SaveData()
	{
		PartyLevel = 1;
		XP = 0;
		Floor = 0;
		Gold = 0;
		Inventory = new Dictionary<string, int>();
		WildCards = new Dictionary<string, int>();
		SavedPosition = Vector2.Zero;
		HasSavedPosition = false;
	}

	// Constructor from dictionary (used for loading)
	public SaveData(Dictionary<string, Variant> data)
	{
		var loaded = FromDictionary(data);
		PartyLevel = loaded.PartyLevel;
		XP = loaded.XP;
		Floor = loaded.Floor;
		Gold = loaded.Gold;
		Inventory = loaded.Inventory;
		WildCards = loaded.WildCards;
		SavedPosition = loaded.SavedPosition;
		HasSavedPosition = loaded.HasSavedPosition;
	}

	// Convert SaveData to Dictionary (for saving)
	public Dictionary<string, Variant> ToDictionary()
	{
		var dict = new Dictionary<string, Variant>
		{
			{"PartyLevel", PartyLevel},
			{"XP", XP},
			{"Floor", Floor},
			{"Gold", Gold},
			{"Inventory", ConvertToGodotDict(Inventory)},
			{"WildCards", ConvertToGodotDict(WildCards)},
			{"SavedPositionX", SavedPosition.X},
			{"SavedPositionY", SavedPosition.Y},
			{"HasSavedPosition", HasSavedPosition}
		};
		return dict;
	}

	// Deserialize SaveData from Dictionary
	public static SaveData FromDictionary(Dictionary<string, Variant> dict)
	{
		var saveData = new SaveData();
		
		if (dict.ContainsKey("PartyLevel"))
			saveData.PartyLevel = dict["PartyLevel"].AsInt32();
		
		if (dict.ContainsKey("XP"))
			saveData.XP = dict["XP"].AsInt32();
		
		if (dict.ContainsKey("Floor"))
			saveData.Floor = dict["Floor"].AsInt32();
		
		if (dict.ContainsKey("Gold"))
			saveData.Gold = dict["Gold"].AsInt32();
		
		if (dict.ContainsKey("Inventory"))
			saveData.Inventory = ConvertDictionaryToItemDict(dict["Inventory"]);
		
		if (dict.ContainsKey("WildCards"))
			saveData.WildCards = ConvertDictionaryToItemDict(dict["WildCards"]);
		
		if (dict.ContainsKey("SavedPositionX") && dict.ContainsKey("SavedPositionY"))
		{
			float x = dict["SavedPositionX"].AsSingle();
			float y = dict["SavedPositionY"].AsSingle();
			saveData.SavedPosition = new Vector2(x, y);
		}
		
		if (dict.ContainsKey("HasSavedPosition"))
			saveData.HasSavedPosition = dict["HasSavedPosition"].AsBool();

		return saveData;
	}

	// Convert Dictionary<string, int> to Godot Dictionary
	private static Godot.Collections.Dictionary ConvertToGodotDict(Dictionary<string, int> dict)
	{
		var godotDict = new Godot.Collections.Dictionary();
		foreach (var pair in dict)
		{
			godotDict[pair.Key] = pair.Value;
		}
		return godotDict;
	}

	// Convert Variant to Dictionary<string, int>
	private static Dictionary<string, int> ConvertDictionaryToItemDict(Variant rawDict)
	{
		if (rawDict.VariantType != Variant.Type.Dictionary)
			return new Dictionary<string, int>();

		var godotDict = rawDict.AsGodotDictionary();
		var result = new Dictionary<string, int>();

		foreach (var key in godotDict.Keys)
		{
			if (key.VariantType == Variant.Type.String && godotDict[key].VariantType == Variant.Type.Int)
			{
				result[key.AsString()] = godotDict[key].AsInt32();
			}
		}

		return result;
	}
}
