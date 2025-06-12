using Godot;
using System;
using System.Collections.Generic;

public class SaveData
{
	// Basic stats
	public int PartyLevel;
	public int XP;
	public int Floor;
	public int CurrentHP;
	public int CurrentSP;
	public int Gold;
	public Dictionary<string, int> Inventory;
	public Dictionary<string, int> WildCards;

	// Default constructor (used for creating new save)
	public SaveData()
	{
		PartyLevel = 1;
		XP = 0;
		Floor = 0;
		Gold = 0;
		Inventory = new Dictionary<string, int>();
		WildCards = new Dictionary<string, int>();
		
	}

	// Constructor from Godot dictionary (used for loading)
	public SaveData(Godot.Collections.Dictionary data)
	{
		var loaded = FromDictionary(data);
		PartyLevel = loaded.PartyLevel;
		XP = loaded.XP;
		Floor = loaded.Floor;
		Gold = loaded.Gold;
		CurrentHP = loaded.CurrentHP;
		CurrentSP = loaded.CurrentSP;
		Inventory = loaded.Inventory;
		WildCards = loaded.WildCards;
	
	}

	// Convert SaveData to Godot Dictionary (for saving)
	public Godot.Collections.Dictionary ToDictionary()
	{
		var dict = new Godot.Collections.Dictionary
		{
			["party_level"] = PartyLevel,
			["xp"] = XP,
			["floor"] = Floor,
			["current_hp"] = CurrentHP,
			["current_sp"] = CurrentSP,
			["gold"] = Gold,
			["inventory"] = ConvertToGodotDict(Inventory),
			["wild_cards"] = ConvertToGodotDict(WildCards),
		};
		return dict;
	}

	// Deserialize SaveData from Godot Dictionary
	public static SaveData FromDictionary(Godot.Collections.Dictionary data)
	{
		var saveData = new SaveData
		{
			PartyLevel = (int)data["party_level"],
			XP = (int)data["xp"],
			Floor = (int)data["floor"],
			CurrentHP = (int)data["current_hp"],
			CurrentSP = (int)data["current_sp"],
			Gold = (int)data["gold"],
			Inventory = ConvertDictionaryToItemDict(data["inventory"]),
			WildCards = ConvertDictionaryToItemDict(data["wild_cards"]),
		};
		return saveData;
	}

	// Convert Dictionary<string, Dictionary<string, int>> to Godot Dictionary
	private static Godot.Collections.Dictionary ConvertToGodotDict(Dictionary<string, Dictionary<string, int>> dict)
	{
		var godotDict = new Godot.Collections.Dictionary();
		foreach (var pair in dict)
		{
			var inner = new Godot.Collections.Dictionary();
			foreach (var sub in pair.Value)
			{
				inner[sub.Key] = sub.Value;
			}
			godotDict[pair.Key] = inner;
		}
		return godotDict;
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
	private static Dictionary<string, int> ConvertDictionaryToItemDict(object rawDict)
	{
		var godotDict = ((Variant)rawDict).AsGodotDictionary();
		var result = new Dictionary<string, int>();

		foreach (var key in godotDict.Keys)
		{
			result[key.ToString()] = (int)(Variant)godotDict[key];
		}

		return result;
	}
}
