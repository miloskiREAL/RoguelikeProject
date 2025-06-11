using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : Character
{
	[Export] public EnemyClass Class;
	public enum EnemyClass
	{
		Skeleton,
		Gravetender,
		Goblin,
		Minotaur,
		GoblinArcher,
		GoblinMage,
		Necromancer,
		DarkKnight
	}

	public void Init(EnemyClass enemyClass)
	{
		CharacterName = enemyClass.ToString();

		switch (enemyClass)
		{
			case EnemyClass.Skeleton:
				MaxHP = 100;
				MaxSP = 30;
				Strength = 15;
				Magic = 5;
				Defense = 10;
				Speed = 8;
				Weaknesses.Add(ElementType.Holy);
				Resistances.Add(ElementType.Darkness);
				Skills.Add(new Skill { Name = "Bone Smash", Power = 40, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.Gravetender:
				MaxHP = 120;
				MaxSP = 40;
				Strength = 10;
				Magic = 15;
				Defense = 12;
				Speed = 7;
				Weaknesses.Add(ElementType.Fire);
				Resistances.Add(ElementType.Darkness);
				Skills.Add(new Skill { Name = "Dark Pulse", Power = 50, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.Goblin:
				MaxHP = 90;
				MaxSP = 20;
				Strength = 20;
				Magic = 5;
				Defense = 8;
				Speed = 12;
				Skills.Add(new Skill { Name = "Stab", Power = 35, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.Minotaur:
				MaxHP = 180;
				MaxSP = 40;
				Strength = 28;
				Magic = 5;
				Defense = 18;
				Speed = 10;
				Weaknesses.Add(ElementType.Ice);
				Skills.Add(new Skill { Name = "Bull Rush", Power = 60, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.GoblinArcher:
				MaxHP = 85;
				MaxSP = 30;
				Strength = 15;
				Magic = 10;
				Defense = 6;
				Speed = 16;
				Skills.Add(new Skill { Name = "Arrow Shot", Power = 40, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Wind, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.GoblinMage:
				MaxHP = 80;
				MaxSP = 60;
				Strength = 5;
				Magic = 25;
				Defense = 6;
				Speed = 12;
				Weaknesses.Add(ElementType.Neutral);
				Skills.Add(new Skill { Name = "Fireball", Power = 45, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Fire, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.Necromancer:
				MaxHP = 140;
				MaxSP = 80;
				Strength = 5;
				Magic = 30;
				Defense = 10;
				Speed = 10;
				Weaknesses.Add(ElementType.Holy);
				Resistances.Add(ElementType.Darkness);
				Skills.Add(new Skill { Name = "Shadow Bind", Power = 60, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				break;

			case EnemyClass.DarkKnight:
				MaxHP = 200;
				MaxSP = 50;
				Strength = 25;
				Magic = 15;
				Defense = 20;
				Speed = 14;
				Weaknesses.Add(ElementType.Holy);
				Resistances.Add(ElementType.Neutral);
				Skills.Add(new Skill { Name = "Dark Slash", Power = 70, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Darkness, Cost = 0, Unlocked = true });
				break;
		}

		CurrentHP = MaxHP;
		CurrentSP = MaxSP;
	}

	public void PerformAI(List<PlayerCharacter> playerParty)
	{
		if (IsDead()) return;

		Skill skill = Skills[rng.Next(Skills.Count)];
		Character target;

		if (skill.Targeting == TargetType.AllEnemies)
		{
			foreach (var p in playerParty)
			{
				if (!p.IsDead())
					skill.Activate(this, p);
			}
		}
		else
		{
			List<PlayerCharacter> aliveTargets = playerParty.FindAll(p => !p.IsDead());
			if (aliveTargets.Count == 0) return;
			target = aliveTargets[rng.Next(aliveTargets.Count)];
			skill.Activate(this, target);
		}
	}
}
