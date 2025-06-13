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

	public void UseOn(Character target)
	{
		SingleTargetEffect?.Invoke(target);
	}

	public void UseOn(List<Character> targets)
	{
		MultiTargetEffect?.Invoke(targets);
	}

	public static Dictionary<string, Item> All = new()
	{
		{
			"HP_Potion_Single",
			new Item(
				"HP_Potion_Single",
				"HP Potion",
				"Restores 50 HP to one ally.",
				(target) => {
					target.Heal(50);
				}
			)
		},
		{
			"SP_Potion_Single",
			new Item(
				"SP_Potion_Single",
				"SP Potion",
				"Restores 30 SP to one ally.",
				(target) => {
					int restored = Math.Min(30, target.MaxSP - target.CurrentSP);
					target.CurrentSP = Math.Min(target.MaxSP, target.CurrentSP + 30);
					// Note: ActivityIndicator messages would need to be handled in BattleManager
				}
			)
		},
		{
			"HP_Potion_Team",
			new Item(
				"HP_Potion_Team",
				"Team HP Potion",
				"Restores 30 HP to all allies.",
				null,
				true,
				(targets) => targets.ForEach(t => t.Heal(30))
			)
		},
		{
			"SP_Potion_Team",
			new Item(
				"SP_Potion_Team",
				"Team SP Potion",
				"Restores 15 SP to all allies.",
				null,
				true,
				(targets) => targets.ForEach(t => t.CurrentSP = Math.Min(t.MaxSP, t.CurrentSP + 15))
			)
		},
		{
			"Full_HP_Single",
			new Item(
				"Full_HP_Single",
				"Full Heal",
				"Fully heals one ally.",
				(target) => target.Heal(target.MaxHP)
			)
		},
		{
			"Full_SP_Single",
			new Item(
				"Full_SP_Single",
				"Full SP",
				"Fully restores SP to one ally.",
				(target) => target.CurrentSP = target.MaxSP
			)
		},
		{
			"Full_HP_Team",
			new Item(
				"Full_HP_Team",
				"Full Heal All",
				"Fully heals all allies.",
				null,
				true,
				(targets) => targets.ForEach(t => t.Heal(t.MaxHP))
			)
		},
		{
			"Full_SP_Team",
			new Item(
				"Full_SP_Team",
				"Full SP All",
				"Fully restores SP to all allies.",
				null,
				true,
				(targets) => targets.ForEach(t => t.CurrentSP = t.MaxSP)
			)
		},
		{
			"Full_Restore_Team",
			new Item(
				"Full_Restore_Team",
				"Elixir",
				"Fully restores HP and SP to all allies.",
				null,
				true,
				(targets) =>
				{
					foreach (var t in targets)
					{
						t.Heal(t.MaxHP);
						t.CurrentSP = t.MaxSP;
					}
				}
			)
		},
	};
}
