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
	
	// Position data
	public Vector2 SavedPosition { get; set; } = Vector2.Zero;
	public bool HasSavedPosition { get; set; } = false;
	
	// Current dungeon layout stores which random layout we're currently in
	public int CurrentDungeonLayout { get; set; } = 0; // 0 means not set, 1-4 are valid layouts

	// Boss trip wire tracking, key is "floor_block" (e.g. "5_2" for floor 5 block 2 boss)
	public Dictionary<string, bool> TriggeredBossWires { get; set; } = new Dictionary<string, bool>();

	// DEPRECATED - keeping for backward compatibility but not used
	public HashSet<string> OpenedChests { get; set; } = new HashSet<string>();
	
	// Chest tracking that resets each floor
	public HashSet<string> OpenedChestsCurrentFloor { get; set; } = new HashSet<string>();
	private int _lastFloor = -1; // Track floor changes to clear chests

	// Default constructor
	public SaveData()
	{
		PartyLevel = 1;
		XP = 0;
		Floor = 0;
		Gold = 100;
		Inventory = new Dictionary<string, int>();
		WildCards = new Dictionary<string, int>();
		SavedPosition = Vector2.Zero;
		HasSavedPosition = false;
		CurrentDungeonLayout = 0;
		TriggeredBossWires = new Dictionary<string, bool>();
		OpenedChests = new HashSet<string>();
		OpenedChestsCurrentFloor = new HashSet<string>();
		_lastFloor = -1;
	}

	// Constructor from dictionary used for loading save data
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
		CurrentDungeonLayout = loaded.CurrentDungeonLayout;
		TriggeredBossWires = loaded.TriggeredBossWires;
		OpenedChests = loaded.OpenedChests;
		OpenedChestsCurrentFloor = loaded.OpenedChestsCurrentFloor;
		_lastFloor = loaded._lastFloor;
	}

	// Checks if floor changed and clears current floor chests if so
	public void CheckFloorChange()
	{
		if (_lastFloor != -1 && _lastFloor != Floor)
		{
			OpenedChestsCurrentFloor.Clear();
		}
		_lastFloor = Floor;
	}

	// Converts SaveData to Dictionary for saving to file
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
			{"HasSavedPosition", HasSavedPosition},
			{"CurrentDungeonLayout", CurrentDungeonLayout},
			{"TriggeredBossWires", ConvertBoolDictToGodotDict(TriggeredBossWires)},
			{"OpenedChests", ConvertHashSetToGodotArray(OpenedChests)},
			{"OpenedChestsCurrentFloor", ConvertHashSetToGodotArray(OpenedChestsCurrentFloor)},
			{"LastFloor", _lastFloor}
		};
		
		return dict;
	}

	// Deserializes SaveData from Dictionary loaded from file
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
			saveData.Inventory = ConvertVariantToItemDict(dict["Inventory"]);
		
		if (dict.ContainsKey("WildCards"))
			saveData.WildCards = ConvertVariantToItemDict(dict["WildCards"]);
		
		if (dict.ContainsKey("SavedPositionX") && dict.ContainsKey("SavedPositionY"))
		{
			float x = dict["SavedPositionX"].AsSingle();
			float y = dict["SavedPositionY"].AsSingle();
			saveData.SavedPosition = new Vector2(x, y);
		}
		
		if (dict.ContainsKey("HasSavedPosition"))
			saveData.HasSavedPosition = dict["HasSavedPosition"].AsBool();
		
		if (dict.ContainsKey("CurrentDungeonLayout"))
			saveData.CurrentDungeonLayout = dict["CurrentDungeonLayout"].AsInt32();

		if (dict.ContainsKey("TriggeredBossWires"))
			saveData.TriggeredBossWires = ConvertVariantToBoolDict(dict["TriggeredBossWires"]);

		if (dict.ContainsKey("OpenedChests"))
			saveData.OpenedChests = ConvertVariantToHashSet(dict["OpenedChests"]);

		if (dict.ContainsKey("OpenedChestsCurrentFloor"))
			saveData.OpenedChestsCurrentFloor = ConvertVariantToHashSet(dict["OpenedChestsCurrentFloor"]);

		if (dict.ContainsKey("LastFloor"))
			saveData._lastFloor = dict["LastFloor"].AsInt32();

		return saveData;
	}

	// Convert Dictionary<string, int> to Godot Dictionary
	private static Godot.Collections.Dictionary ConvertToGodotDict(Dictionary<string, int> dict)
	{
		var godotDict = new Godot.Collections.Dictionary();
		if (dict != null)
		{
			foreach (var pair in dict)
			{
				godotDict[pair.Key] = pair.Value;
			}
		}
		return godotDict;
	}

	// Convert Dictionary<string, bool> to Godot Dictionary
	private static Godot.Collections.Dictionary ConvertBoolDictToGodotDict(Dictionary<string, bool> dict)
	{
		var godotDict = new Godot.Collections.Dictionary();
		if (dict != null)
		{
			foreach (var pair in dict)
			{
				godotDict[pair.Key] = pair.Value;
			}
		}
		return godotDict;
	}

	// Convert HashSet<string> to Godot Array
	private static Godot.Collections.Array ConvertHashSetToGodotArray(HashSet<string> hashSet)
	{
		var godotArray = new Godot.Collections.Array();
		if (hashSet != null)
		{
			foreach (var item in hashSet)
			{
				godotArray.Add(item);
			}
		}
		return godotArray;
	}

	// Convert Variant to Dictionary<string, int>
	private static Dictionary<string, int> ConvertVariantToItemDict(Variant rawDict)
	{
		var result = new Dictionary<string, int>();
		
		if (rawDict.VariantType != Variant.Type.Dictionary)
			return result;

		var godotDict = rawDict.AsGodotDictionary();
		
		foreach (var key in godotDict.Keys)
		{
			string keyStr = key.AsString();
			int value = godotDict[key].AsInt32();
			result[keyStr] = value;
		}

		return result;
	}

	// Convert Variant to Dictionary<string, bool>
	private static Dictionary<string, bool> ConvertVariantToBoolDict(Variant rawDict)
	{
		var result = new Dictionary<string, bool>();
		
		if (rawDict.VariantType != Variant.Type.Dictionary)
			return result;

		var godotDict = rawDict.AsGodotDictionary();

		foreach (var key in godotDict.Keys)
		{
			string keyStr = key.AsString();
			bool value = godotDict[key].AsBool();
			result[keyStr] = value;
		}

		return result;
	}

	// Convert Variant to HashSet<string>
	private static HashSet<string> ConvertVariantToHashSet(Variant rawArray)
	{
		var result = new HashSet<string>();
		
		if (rawArray.VariantType != Variant.Type.Array)
			return result;

		var godotArray = rawArray.AsGodotArray();

		foreach (var item in godotArray)
		{
			string itemStr = item.AsString();
			result.Add(itemStr);
		}

		return result;
	}

	// Adds item to inventory or increases quantity if already exists
	public void AddItem(string itemKey, int quantity)
	{
		if (Inventory.ContainsKey(itemKey))
		{
			Inventory[itemKey] += quantity;
		}
		else
		{
			Inventory[itemKey] = quantity;
		}
	}

	// Adds gold to player's total
	public void AddGold(int amount)
	{
		Gold += amount;
	}
}
