using System;
using System.Collections.Generic;

public class Item
{
	public string Key;                        
	public string Name;                        
	public string Description;                 
	public bool IsTeamWide;                   

	public Action<Character> SingleTargetEffect;          
	public Action<List<Character>> MultiTargetEffect;     

	public Item(string key, string name, string description,
				Action<Character> singleTargetEffect = null,
				bool isTeamWide = false,
				Action<List<Character>> multiTargetEffect = null)
	{
		Key = key;
		Name = name;
		Description = description;
		SingleTargetEffect = singleTargetEffect;
		IsTeamWide = isTeamWide;
		MultiTargetEffect = multiTargetEffect;
	}
//These are the activation functions for the items 
	public void UseOn(Character target)
	{
		SingleTargetEffect?.Invoke(target);
	}
	
	public void UseOn(List<Character> targets)
	{
		MultiTargetEffect?.Invoke(targets);
	}
	// All of the item definitions, has a key, name, desc, and effect type
	public static Dictionary<string, Item> All = new()
	{
		{
			"Potion",
			new Item(
				"Potion",
				"Potion",
				"Restores 50 HP to one ally.",
				(target) => {
					if (target != null)
					{
						target.Heal(50);
					}
				}
			)
		},
		{
			"Ether",
			new Item(
				"Ether",
				"Ether",
				"Restores 30 SP to one ally.",
				(target) => {
					if (target != null)
					{
						int restored = Math.Min(30, target.MaxSP - target.CurrentSP);
						target.CurrentSP = Math.Min(target.MaxSP, target.CurrentSP + 30);
					}
				}
			)
		},
		{
			"Mega Potion",
			new Item(
				"Mega Potion",
				"Mega Potion",
				"Restores 30 HP to all allies.",
				null,
				true, 
				(targets) => {
					if (targets != null)
					{
						foreach (var target in targets)
						{
							if (target != null)
							{
								target.Heal(30);
							}
						}
					}
				}
			)
		},
		{
			"Mega Ether",
			new Item(
				"Mega Ether",
				"Mega Ether",
				"Restores 15 SP to all allies.",
				null, 
				true, 
				(targets) => {
					if (targets != null)
					{
						foreach (var target in targets)
						{
							if (target != null)
							{
								target.CurrentSP = Math.Min(target.MaxSP, target.CurrentSP + 15);
							}
						}
					}
				}
			)
		},
		{
			"Full Heal",
			new Item(
				"Full Heal",
				"Full Heal",
				"Fully heals one ally.",
				(target) => {
					if (target != null)
					{
						target.Heal(target.MaxHP);
					}
				}
			)
		},
		{
			"Full Ether",
			new Item(
				"Full Ether",
				"Full SP",
				"Fully restores SP to one ally.",
				(target) => {
					if (target != null)
					{
						target.CurrentSP = target.MaxSP;
					}
				}
			)
		},
		{
			"Team Heal",
			new Item(
				"Team Heal",
				"Team Heal",
				"Fully heals all allies.",
				null,
				true,
				(targets) => {
					if (targets != null)
					{
						foreach (var target in targets)
						{
							if (target != null)
							{
								target.Heal(target.MaxHP);
							}
						}
					}
				}
			)
		},
		{
			"Team Ether",
			new Item(
				"Team Ether",
				"Team Ether",
				"Fully restores SP to all allies.",
				null, 
				true, 
				(targets) => {
					if (targets != null)
					{
						foreach (var target in targets)
						{
							if (target != null)
							{
								target.CurrentSP = target.MaxSP;
							}
						}
					}
				}
			)
		},
		{
			"Elixer",
			new Item(
				"Elixer",
				"Elixir",
				"Fully restores HP and SP to all allies.",
				null,
				true, 
				(targets) => {
					if (targets != null)
					{
						foreach (var target in targets)
						{
							if (target != null)
							{
								target.Heal(target.MaxHP);
								target.CurrentSP = target.MaxSP;
							}
						}
					}
				}
			)
		},
	};
}
