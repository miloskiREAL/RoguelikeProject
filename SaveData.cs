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

	// Constructor to initialize default values
	public SaveData()
	{
		Inventory = new Dictionary<string, int>();
		WildCards = new Dictionary<string, int>();
		AlteredAffinities = new Dictionary<string, Dictionary<string, int>>();
	}

	// Convert a SaveData object to a Godot Dictionary for easy JSON serialization
	public Godot.Collections.Dictionary ToDictionary()
	{
		var dict = new Godot.Collections.Dictionary();
		dict["party_level"] = PartyLevel;
		dict["xp"] = XP;
		dict["floor"] = Floor;
		dict["current_hp"] = CurrentHP;
		dict["current_sp"] = CurrentSP;
		dict["inventory"] = ConvertToGodotDict(Inventory);
		dict["wild_cards"] = ConvertToGodotDict(WildCards);
		dict["altered_affinities"] = ConvertToGodotDict(AlteredAffinities);
		return dict;
	}

	// Convert a Godot Dictionary back into a SaveData object
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

	//realized I needed to convert C# dict to normal dict asked gpt for help
	private static Dictionary<string, Dictionary<string, int>> ConvertDictionaryToAffinities(object rawDict)
	{
		var godotDict = rawDict as Godot.Collections.Dictionary;
		var result = new Dictionary<string, Dictionary<string, int>>();
		
		foreach (var key in godotDict.Keys)
		{
			string character = key.ToString();
			var elementAffinities = godotDict[key] as Godot.Collections.Dictionary;
			var affinityDict = new Dictionary<string, int>();

			foreach (var elementKey in elementAffinities.Keys)
			{
				affinityDict[elementKey.ToString()] = (int)elementAffinities[elementKey];
			}

			result[character] = affinityDict;
		}

		return result;
	}

	// Helper to convert regular Dictionaries back to Godot format for serialization
	private static Godot.Collections.Dictionary ConvertToGodotDict(Dictionary<string, Dictionary<string, int>> dict)
	{
		var godotDict = new Godot.Collections.Dictionary();
		foreach (var pair in dict)
		{
			var affinityDict = new Godot.Collections.Dictionary();
			foreach (var subPair in pair.Value)
			{
				affinityDict[subPair.Key] = subPair.Value;
			}
			godotDict[pair.Key] = affinityDict;
		}
		return godotDict;
	}

	// Helper to convert a regular Dictionary<string, int> to Godot format for serialization
	private static Godot.Collections.Dictionary ConvertToGodotDict(Dictionary<string, int> dict)
	{
		var godotDict = new Godot.Collections.Dictionary();
		foreach (var pair in dict)
		{
			godotDict[pair.Key] = pair.Value;
		}
		return godotDict;
	}

	// Helper to convert a Godot Dictionary back to a regular Dictionary<string, int>
	private static Dictionary<string, int> ConvertDictionaryToItemDict(object rawDict)
	{
		var godotDict = rawDict as Godot.Collections.Dictionary;
		var result = new Dictionary<string, int>();
		
		foreach (var key in godotDict.Keys)
		{
			result[key.ToString()] = (int)godotDict[key];
		}

		return result;
	}
}
