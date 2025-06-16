using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Enemy : Character
{
	[Export] public EnemyClass Class;
	public enum EnemyClass
	{
		Skeleton,
		Gravetender,
		Goblin,
		GoblinArcher,
		HammerGoblin,
		Minotaur,
		Necromancer,
		DarkKnight,
		Asterius,
		TheLich,
		Kingslayer, 
		FellGod
	}
	// Initializes similar to player characters
	public void Init(EnemyClass enemyClass)
	{
		CharacterName = enemyClass.ToString();
		Class = enemyClass;
		int partyLevel = GameManager.Instance?.SaveData?.PartyLevel ?? 1;

		Weaknesses.Clear();
		Resistances.Clear();
		Skills.Clear();

		CriticalChance = 10; 
		DamageReduction = 5; 
		switch (enemyClass)
		{
			case EnemyClass.Skeleton:
				MaxHP = 120 + (int)(partyLevel * 1.5); 
				MaxSP = 45 + (int)(partyLevel * 1.2); 
				Strength = 18 + (int)(partyLevel * 1.2); 
				Magic = 8 + (int)(partyLevel * 0.8); 
				Defense = 12 + (int)(partyLevel * 0.9);
				Speed = 12 + (int)(partyLevel * 1.0); 

				CriticalChance = 12;
				DamageReduction = 8;

				Weaknesses.Add(ElementType.Holy);
				Weaknesses.Add(ElementType.Fire);
				Resistances.Add(ElementType.Darkness);
				Resistances.Add(ElementType.Ice);
				Resistances.Add(ElementType.Neutral);

				Skills.Add(new Skill { Name = "Bone Smash", Power = 60, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Bone Throw", Power = 45, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Rattle", Power = 0, Type = SkillType.Debuff, Targeting = TargetType.AllEnemies, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Bone Spear", Power = 50, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.Gravetender:
				MaxHP = 150 + (int)(partyLevel * 1.6);
				MaxSP = 60 + (int)(partyLevel * 1.5);
				Strength = 15 + (int)(partyLevel * 1.0);
				Magic = 23 + (int)(partyLevel * 1.5);
				Defense = 15 + (int)(partyLevel * 1.0);
				Speed = 11 + (int)(partyLevel * 0.9);

				CriticalChance = 15;
				DamageReduction = 10;

				Weaknesses.Add(ElementType.Fire);
				Weaknesses.Add(ElementType.Holy);
				Resistances.Add(ElementType.Darkness);
				Resistances.Add(ElementType.Earth);
				Resistances.Add(ElementType.Ice);

				Skills.Add(new Skill { Name = "Dark Pulse", Power = 75, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Necrotic Drain", Power = 50, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Soul Whisper", Power = 0, Type = SkillType.Debuff, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Grave Mist", Power = 40, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.Goblin:
				MaxHP = 128 + (int)(partyLevel * 1.8);
				MaxSP = 38 + (int)(partyLevel * 1.4);
				Strength = 27 + (int)(partyLevel * 1.5);
				Magic = 8 + (int)(partyLevel * 0.8);
				Defense = 9 + (int)(partyLevel * 1.0);
				Speed = 18 + (int)(partyLevel * 1.2);

				CriticalChance = 18;
				DamageReduction = 6;

				Weaknesses.Add(ElementType.Holy);
				Weaknesses.Add(ElementType.Lightning);
				Resistances.Add(ElementType.Earth);
				Resistances.Add(ElementType.Neutral);
				Resistances.Add(ElementType.Darkness);

				Skills.Add(new Skill { Name = "Stab", Power = 53, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Dirty Fighting", Power = 38, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "War Cry", Power = 0, Type = SkillType.Buff, Targeting = TargetType.AllAllies, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Frenzy Attack", Power = 30, Type = SkillType.Physical, Targeting = TargetType.AllEnemies, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.GoblinArcher:
				MaxHP = 120 + (int)(partyLevel * 2.0);
				MaxSP = 53 + (int)(partyLevel * 1.7);
				Strength = 21 + (int)(partyLevel * 1.7);
				Magic = 15 + (int)(partyLevel * 1.0);
				Defense = 8 + (int)(partyLevel * 0.9);
				Speed = 24 + (int)(partyLevel * 1.4);

				CriticalChance = 22;
				DamageReduction = 5;

				Weaknesses.Add(ElementType.Fire);
				Weaknesses.Add(ElementType.Earth);
				Resistances.Add(ElementType.Wind);
				Resistances.Add(ElementType.Neutral);
				Resistances.Add(ElementType.Ice);

				Skills.Add(new Skill { Name = "Arrow Shot", Power = 60, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Wind, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Piercing Shot", Power = 83, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Multi Shot", Power = 45, Type = SkillType.Physical, Targeting = TargetType.AllEnemies, Element = ElementType.Wind, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Wind Arrow", Power = 70, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Wind, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.HammerGoblin:
				MaxHP = 113 + (int)(partyLevel * 2.1);
				MaxSP = 75 + (int)(partyLevel * 1.8);
				Strength = 33 + (int)(partyLevel * 1.8);
				Magic = 12 + (int)(partyLevel * 0.9);
				Defense = 12 + (int)(partyLevel * 1.2);
				Speed = 15 + (int)(partyLevel * 1.0);

				CriticalChance = 16;
				DamageReduction = 12;

				Weaknesses.Add(ElementType.Ice);
				Weaknesses.Add(ElementType.Wind);
				Resistances.Add(ElementType.Fire);
				Resistances.Add(ElementType.Earth);
				Resistances.Add(ElementType.Neutral);

				Skills.Add(new Skill { Name = "Fireball", Power = 90, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Fire, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Hammer Slam", Power = 105, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Earth, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Flame Burst", Power = 68, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Fire, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Meteor Strike", Power = 80, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Fire, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.Minotaur:
				MaxHP = 240 + (int)(partyLevel * 2.3);
				MaxSP = 68 + (int)(partyLevel * 1.7);
				Strength = 38 + (int)(partyLevel * 2.0);
				Magic = 8 + (int)(partyLevel * 0.8);
				Defense = 24 + (int)(partyLevel * 1.5);
				Speed = 12 + (int)(partyLevel * 0.9);

				CriticalChance = 20;
				DamageReduction = 15;

				Weaknesses.Add(ElementType.Ice);
				Weaknesses.Add(ElementType.Lightning);
				Resistances.Add(ElementType.Earth);
				Resistances.Add(ElementType.Fire);
				Resistances.Add(ElementType.Neutral);

				Skills.Add(new Skill { Name = "Bull Rush", Power = 90, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Earthquake Stomp", Power = 75, Type = SkillType.Physical, Targeting = TargetType.AllEnemies, Element = ElementType.Earth, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Intimidating Roar", Power = 0, Type = SkillType.Debuff, Targeting = TargetType.AllEnemies, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Berserker Fury", Power = 0, Type = SkillType.Buff, Targeting = TargetType.SingleAlly, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Horn Gore", Power = 110, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.Necromancer:
				MaxHP = 180 + (int)(partyLevel * 2.1);
				MaxSP = 105 + (int)(partyLevel * 2.4);
				Strength = 12 + (int)(partyLevel * 0.8);
				Magic = 42 + (int)(partyLevel * 2.1);
				Defense = 18 + (int)(partyLevel * 1.2);
				Speed = 18 + (int)(partyLevel * 1.0);

				CriticalChance = 25;
				DamageReduction = 18;

				Weaknesses.Add(ElementType.Holy);
				Weaknesses.Add(ElementType.Neutral);
				Resistances.Add(ElementType.Darkness);
				Resistances.Add(ElementType.Ice);
				Resistances.Add(ElementType.Earth);

				Skills.Add(new Skill { Name = "Shadow Bind", Power = 90, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Death Coil", Power = 120, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Curse of Weakness", Power = 0, Type = SkillType.Debuff, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Summon Undead", Power = 0, Type = SkillType.Buff, Targeting = TargetType.AllAllies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Dark Healing", Power = 60, Type = SkillType.Heal, Targeting = TargetType.SingleAlly, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.DarkKnight:
				MaxHP = 270 + (int)(partyLevel * 2.4);
				MaxSP = 90 + (int)(partyLevel * 2.0);
				Strength = 33 + (int)(partyLevel * 2.0);
				Magic = 23 + (int)(partyLevel * 1.5);
				Defense = 27 + (int)(partyLevel * 1.7);
				Speed = 21 + (int)(partyLevel * 1.2);

				CriticalChance = 22;
				DamageReduction = 20;

				Weaknesses.Add(ElementType.Holy);
				Weaknesses.Add(ElementType.Lightning);
				Resistances.Add(ElementType.Darkness);
				Resistances.Add(ElementType.Neutral);
				Resistances.Add(ElementType.Fire);

				Skills.Add(new Skill { Name = "Dark Slash", Power = 105, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Shadow Strike", Power = 128, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Dark Aura", Power = 0, Type = SkillType.Buff, Targeting = TargetType.AllAllies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Void Shield", Power = 0, Type = SkillType.Buff, Targeting = TargetType.SingleAlly, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Shadow Eruption", Power = 85, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.Asterius:
				MaxHP = 600 + (int)(partyLevel * 3.8);
				MaxSP = 120 + (int)(partyLevel * 2.7);
				Strength = 53 + (int)(partyLevel * 2.7);
				Magic = 18 + (int)(partyLevel * 1.2);
				Defense = 38 + (int)(partyLevel * 2.1);
				Speed = 18 + (int)(partyLevel * 1.2);

				CriticalChance = 25;
				DamageReduction = 25;

				Weaknesses.Add(ElementType.Ice);
				Weaknesses.Add(ElementType.Lightning);
				Resistances.Add(ElementType.Earth);
				Resistances.Add(ElementType.Fire);
				Resistances.Add(ElementType.Neutral);
				Resistances.Add(ElementType.Darkness);

				Skills.Add(new Skill { Name = "Berserker Rush", Power = 135, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Devastating Stomp", Power = 105, Type = SkillType.Physical, Targeting = TargetType.AllEnemies, Element = ElementType.Earth, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Terrifying Bellow", Power = 0, Type = SkillType.Debuff, Targeting = TargetType.AllEnemies, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Rage", Power = 0, Type = SkillType.Buff, Targeting = TargetType.SingleAlly, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Earthquake Nova", Power = 120, Type = SkillType.Physical, Targeting = TargetType.AllEnemies, Element = ElementType.Earth, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Titan's Wrath", Power = 160, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.TheLich:
				MaxHP = 525 + (int)(partyLevel * 3.3);
				MaxSP = 180 + (int)(partyLevel * 3.6);
				Strength = 23 + (int)(partyLevel * 1.2);
				Magic = 68 + (int)(partyLevel * 3.0);
				Defense = 30 + (int)(partyLevel * 1.8);
				Speed = 27 + (int)(partyLevel * 1.5);

				CriticalChance = 28;
				DamageReduction = 30;

				Weaknesses.Add(ElementType.Holy);
				Resistances.Add(ElementType.Darkness);
				Resistances.Add(ElementType.Ice);
				Resistances.Add(ElementType.Neutral);
				Resistances.Add(ElementType.Earth);

				Skills.Add(new Skill { Name = "Soul Reaper", Power = 165, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Mass Shadow Bind", Power = 120, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Curse of Doom", Power = 0, Type = SkillType.Debuff, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Life Drain", Power = 90, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Dark Regeneration", Power = 120, Type = SkillType.Heal, Targeting = TargetType.SingleAlly, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Necrotic Storm", Power = 100, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Soul Prison", Power = 140, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.Kingslayer:
				MaxHP = 750 + (int)(partyLevel * 4.2);
				MaxSP = 150 + (int)(partyLevel * 3.0);
				Strength = 60 + (int)(partyLevel * 3.0);
				Magic = 38 + (int)(partyLevel * 2.0);
				Defense = 45 + (int)(partyLevel * 2.4);
				Speed = 30 + (int)(partyLevel * 1.7);

				CriticalChance = 30;
				DamageReduction = 35;

				Weaknesses.Add(ElementType.Holy);
				Resistances.Add(ElementType.Darkness);
				Resistances.Add(ElementType.Neutral);
				Resistances.Add(ElementType.Fire);
				Resistances.Add(ElementType.Ice);

				Skills.Add(new Skill { Name = "Void Slash", Power = 180, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Shadow Cleave", Power = 143, Type = SkillType.Physical, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Dark Shield", Power = 0, Type = SkillType.Buff, Targeting = TargetType.AllAllies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Nightmare", Power = 0, Type = SkillType.Debuff, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Soul Burn", Power = 128, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Abyssal Strike", Power = 200, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Dark Nova", Power = 110, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.FellGod:
				MaxHP = 1200 + (int)(partyLevel * 6.0);
				MaxSP = 300 + (int)(partyLevel * 4.5);
				Strength = 75 + (int)(partyLevel * 3.8);
				Magic = 90 + (int)(partyLevel * 4.2);
				Defense = 60 + (int)(partyLevel * 3.0);
				Speed = 38 + (int)(partyLevel * 2.3);

				CriticalChance = 35;
				DamageReduction = 40;

				// Final boss has one weakness but has many resistances 
				Weaknesses.Add(ElementType.Holy);
				Resistances.Add(ElementType.Darkness);
				Resistances.Add(ElementType.Fire);
				Resistances.Add(ElementType.Ice);
				Resistances.Add(ElementType.Lightning);
				Resistances.Add(ElementType.Earth);
				Resistances.Add(ElementType.Wind);

				Skills.Add(new Skill { Name = "Divine Wrath", Power = 225, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Apocalypse", Power = 300, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "World Ender", Power = 195, Type = SkillType.Physical, Targeting = TargetType.AllEnemies, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Eternal Curse", Power = 0, Type = SkillType.Debuff, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Fell Regeneration", Power = 225, Type = SkillType.Heal, Targeting = TargetType.SingleAlly, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "God's Fury", Power = 0, Type = SkillType.Buff, Targeting = TargetType.SingleAlly, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Reality Tear", Power = 250, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				Skills.Add(new Skill { Name = "Cosmic Devastation", Power = 180, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				break;
		}

		CurrentHP = MaxHP;
		CurrentSP = MaxSP;
	}

public void PerformAI(List<PlayerCharacter> playerParty, List<Enemy> enemyParty = null)
{
	if (IsDead()) return;

		// Bosses have smarter skill selection than regular enemies
	Skill skill;
		
	if (Class == EnemyClass.FellGod || Class == EnemyClass.Kingslayer || 
		Class == EnemyClass.TheLich || Class == EnemyClass.Asterius)
	{
			// Prioritize powerful attacks
		var attackSkills = Skills.Where(s => s.Type == SkillType.Physical || s.Type == SkillType.Magical).ToList();
		var supportSkills = Skills.Where(s => s.Type == SkillType.Buff || s.Type == SkillType.Heal || s.Type == SkillType.Debuff).ToList();
			
		if (rng.Next(100) < 70 && attackSkills.Count > 0)
		{
			skill = attackSkills[rng.Next(attackSkills.Count)];
		}
		else if (supportSkills.Count > 0)
		{
			skill = supportSkills[rng.Next(supportSkills.Count)];
		}
		else
		{
			skill = Skills[rng.Next(Skills.Count)];
		}
	}
	else
	{
		skill = Skills[rng.Next(Skills.Count)];
	}

	activityIndicator?.AddMessage($"{CharacterName}'s turn");

	switch (skill.Targeting)
	{
		case TargetType.SingleEnemy:
				// Target player characters
			var alivePlayerTargets = playerParty.Where(p => !p.IsDead()).ToList();
			if (alivePlayerTargets.Count > 0)
			{
				var target = alivePlayerTargets[rng.Next(alivePlayerTargets.Count)];
				skill.Activate(this, target);
			}
			break;

		case TargetType.AllEnemies:
				// Targets all players
			foreach (var player in playerParty.Where(p => !p.IsDead()))
			{
				skill.Activate(this, player);
			}
			break;

		case TargetType.SingleAlly:
			if (enemyParty != null)
			{
				var aliveEnemyTargets = enemyParty.Where(e => !e.IsDead()).ToList();
				if (aliveEnemyTargets.Count > 0)
				{
					var target = aliveEnemyTargets[rng.Next(aliveEnemyTargets.Count)];
					skill.Activate(this, target);
				}
			}
			else
			{
				skill.Activate(this, this);
			}
			break;

		case TargetType.AllAllies:
			if (enemyParty != null)
			{
				foreach (var enemy in enemyParty.Where(e => !e.IsDead()))
				{
					skill.Activate(this, enemy);
				}
			}
			else
			{
				skill.Activate(this, this);
			}
			break;
		}
	}
}
