using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class WildCardSystem
{
	public enum WildCardType
	{
		Health,
		Resource,
		Damage,
		Defense,
		Speed,
		Critical,
		Balanced,
		Ultimate
	}

	public class WildCard
	{
		public string Name { get; }
		public string Description { get; }
		public WildCardType Type { get; }

		public WildCard(string name, string description, WildCardType type)
		{
			Name = name;
			Description = description;
			Type = type;
		}
	}

	public static List<WildCard> GetAllWildCards()
	{
		//These are all of the wildcards that exist, they each have a name, desc, and type defined here
		return new List<WildCard>
		{
			// HP and SP Boosts
			new WildCard("The Star", "+20 Max HP", WildCardType.Health),
			new WildCard("The High Priestess", "+15 Max SP", WildCardType.Resource),
			new WildCard("The Sun", "+30 Max HP", WildCardType.Health),
			new WildCard("The Magician", "+25 Max SP", WildCardType.Resource),

			// Damage Stats
			new WildCard("Strength", "+5 Strength", WildCardType.Damage),
			new WildCard("The Hermit", "+5 Magic", WildCardType.Damage),
			new WildCard("The Emperor", "+8 Strength", WildCardType.Damage),
			new WildCard("The Hierophant", "+8 Magic", WildCardType.Damage),

			// Defense
			new WildCard("The Hanged Man", "+3 Defense", WildCardType.Defense),
			new WildCard("Temperance", "+5 Defense", WildCardType.Defense),
			new WildCard("The Tower", "+7 Defense", WildCardType.Defense),

			// Speed
			new WildCard("The Fool", "+3 Speed", WildCardType.Speed),
			new WildCard("The Chariot", "+5 Speed", WildCardType.Speed),

			// Critical Hit Chance
			new WildCard("Wheel of Fortune", "+5% Critical Hit Chance", WildCardType.Critical),
			new WildCard("Justice", "+8% Critical Hit Chance", WildCardType.Critical),
			new WildCard("Death", "+12% Critical Hit Chance", WildCardType.Critical),

			// Damage Reduction
			new WildCard("The Moon", "+10% Damage Reduction", WildCardType.Defense),
			new WildCard("The Devil", "+15% Damage Reduction", WildCardType.Defense),

			// Combo Buffs
			new WildCard("The Empress", "+3 Str, +3 Def", WildCardType.Balanced),
			new WildCard("The Lovers", "+3 Mag, +3 Def", WildCardType.Balanced),
			new WildCard("Judgement", "+3 Str, +2 Speed", WildCardType.Balanced),
			new WildCard("The World", "All Bonuses Combined", WildCardType.Ultimate)
		};
	}

	//Applies the specified effect on the character it is applied to
	public static void ApplyWildCardToCharacter(string cardName, Character character)
	{
		switch (cardName)
		{
			// HP and SP Boosts
			case "The Star":
				character.MaxHP += 20;
				break;
			case "The High Priestess":
				character.MaxSP += 15;
				break;
			case "The Sun":
				character.MaxHP += 30;
				break;
			case "The Magician":
				character.MaxSP += 25;
				break;

			// Damage Stats
			case "Strength":
				character.Strength += 5;
				break;
			case "The Hermit":
				character.Magic += 5;
				break;
			case "The Emperor":
				character.Strength += 8;
				break;
			case "The Hierophant":
				character.Magic += 8;
				break;

			// Defense
			case "The Hanged Man":
				character.Defense += 3;
				break;
			case "Temperance":
				character.Defense += 5;
				break;
			case "The Tower":
				character.Defense += 7;
				break;

			// Speed
			case "The Fool":
				character.Speed += 3;
				break;
			case "The Chariot":
				character.Speed += 5;
				break;

			// Critical Hit Chance
			case "Wheel of Fortune":
				character.CriticalChance += 5;
				break;
			case "Justice":
				character.CriticalChance += 8;
				break;
			case "Death":
				character.CriticalChance += 12;
				break;

			// Damage Reduction Buffs
			case "The Moon":
				character.DamageReduction += 10;
				break;
			case "The Devil":
				character.DamageReduction += 15;
				break;

			// Multi Buffs
			case "The Empress":
				character.Strength += 3;
				character.Defense += 3;
				break;
			case "The Lovers":
				character.Magic += 3;
				character.Defense += 3;
				break;
			case "Judgement":
				character.Strength += 3;
				character.Speed += 2;
				break;
			case "The World":
				// All HP/SP bonuses, Sum of The Star + The Sun, Sum of The High Priestess + The Magician
				character.MaxHP += 50;
				character.MaxSP += 40; 
				
				// All damage bonuses, Sum of Strength + The Emperor, Sum of The Hermit + The Hierophant
				character.Strength += 13; 
				character.Magic += 13;
				
				// All defense bonuses, Sum of The Hanged Man + Temperance + The Tower
				character.Defense += 15; 
				
				// All speed bonuses, Sum of The Fool + The Chariot
				character.Speed += 8;
				
				// All critical bonuses, Sum of Wheel of Fortune + Justice + Death
				character.CriticalChance += 25;
				
				// All damage reduction bonuses, Sum of The Moon + The Devil
				character.DamageReduction += 25; 
				break;
		}
	}

	public static void ApplyWildCardToParty(string cardName, List<Character> party)
	{
		foreach (var character in party)
		{
			ApplyWildCardToCharacter(cardName, character);
			
			// Make sure current HP/SP don't exceed new maximums
			character.CurrentHP = Math.Min(character.CurrentHP, character.MaxHP);
			character.CurrentSP = Math.Min(character.CurrentSP, character.MaxSP);
			
			// Update UI if the character has one
			character.UpdateUI();
		}
	}

	public static List<WildCard> GenerateRandomOffers(int count = 3)
	{
		var allCards = GetAllWildCards();
		var random = new Random();
		var gameManager = GameManager.Instance;
		
		// The World card is extremely rare only 1% chance of appearing
		var worldCard = allCards.FirstOrDefault(card => card.Name == "The World");
		var regularCards = allCards.Where(card => card.Name != "The World").ToList();
		
		// Get wild cards the player doesn't already have too many of
		var eligibleRegularCards = regularCards.Where(card => 
		{
			if (gameManager?.SaveData?.WildCards?.ContainsKey(card.Name) == true)
			{
				return gameManager.SaveData.WildCards[card.Name] < 2; // Max 2 of each regular card until they exhaust the options
			}
			return true;
		}).ToList();

		// Check if The World card is eligible maximum of 1 per run
		bool worldCardEligible = false;
		if (worldCard != null)
		{
			if (gameManager?.SaveData?.WildCards?.ContainsKey(worldCard.Name) != true || 
				gameManager.SaveData.WildCards[worldCard.Name] < 1)
			{
				worldCardEligible = true;
			}
		}

		// If we don't have enough eligible regular cards, include all regular cards
		if (eligibleRegularCards.Count < count)
		{
			eligibleRegularCards = regularCards.ToList();
		}

		// Randomly select the requested number of cards
		var selectedCards = new List<WildCard>();
		for (int i = 0; i < count && (eligibleRegularCards.Count > 0 || worldCardEligible); i++)
		{
			// 1 percent chance of world card spawning 
			if (worldCardEligible && random.Next(1, 101) == 1)
			{
				selectedCards.Add(worldCard);
				worldCardEligible = false; // Can only appear once per selection
			}
			else if (eligibleRegularCards.Count > 0)
			{
				int index = random.Next(eligibleRegularCards.Count);
				selectedCards.Add(eligibleRegularCards[index]);
				eligibleRegularCards.RemoveAt(index);
			}
		}

		return selectedCards;
	}
}
