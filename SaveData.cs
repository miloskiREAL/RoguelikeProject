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

	public Dictionary<string, int> Inventory;
	public Dictionary<string, int> WildCards;
	public Dictionary<string, Dictionary<string, int>> AlteredAffinities;

	// Default constructor (used for creating new save)
	public SaveData()
	{
		PartyLevel = 1;
		XP = 0;
		Floor = 0;
		CurrentHP = 100;
		CurrentSP = 100;
		Inventory = new Dictionary<string, int>();
		WildCards = new Dictionary<string, int>();
		AlteredAffinities = new Dictionary<string, Dictionary<string, int>>();
	}

	// Constructor from Godot dictionary (used for loading)
	public SaveData(Godot.Collections.Dictionary data)
	{
		var loaded = FromDictionary(data);
		PartyLevel = loaded.PartyLevel;
		XP = loaded.XP;
		Floor = loaded.Floor;
		CurrentHP = loaded.CurrentHP;
		CurrentSP = loaded.CurrentSP;
		Inventory = loaded.Inventory;
		WildCards = loaded.WildCards;
		AlteredAffinities = loaded.AlteredAffinities;
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
			["inventory"] = ConvertToGodotDict(Inventory),
			["wild_cards"] = ConvertToGodotDict(WildCards),
			["altered_affinities"] = ConvertToGodotDict(AlteredAffinities)
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
			Inventory = ConvertDictionaryToItemDict(data["inventory"]),
			WildCards = ConvertDictionaryToItemDict(data["wild_cards"]),
			AlteredAffinities = ConvertDictionaryToAffinities(data["altered_affinities"])
		};
		return saveData;
	}

	// Convert Variant to Dictionary<string, Dictionary<string, int>>
	private static Dictionary<string, Dictionary<string, int>> ConvertDictionaryToAffinities(object rawDict)
	{
		var godotDict = ((Variant)rawDict).AsGodotDictionary();
		var result = new Dictionary<string, Dictionary<string, int>>();

		foreach (var key in godotDict.Keys)
		{
			string character = key.ToString();
			var elementAffinities = ((Variant)godotDict[key]).AsGodotDictionary();
			var affinityDict = new Dictionary<string, int>();

			foreach (var elementKey in elementAffinities.Keys)
			{
				affinityDict[elementKey.ToString()] = (int)(Variant)elementAffinities[elementKey];
			}

			result[character] = affinityDict;
		}

		return result;
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
