using Godot;
using System;
using System.Collections.Generic;

public partial class Character : Node2D
{
	public enum SkillType { Physical, Magical, Heal, Buff, Debuff }
	public enum TargetType { SingleEnemy, AllEnemies, SingleAlly, AllAllies, Self }
	public enum ElementType { Neutral, Fire, Ice, Lightning, Holy, Wind, Earth, Darkness }

	public string CharacterName;
	public int MaxHP;
	public int CurrentHP;
	public int MaxSP;
	public int CurrentSP;
	public int Strength;
	public int Magic;
	public int Defense;
	public int Speed;
	public bool IsDefending = false;
	public int BuffStacks = 0;
	public List<Skill> Skills = new();
	public HashSet<ElementType> Weaknesses = new();
	public HashSet<ElementType> Resistances = new();

	protected Random rng = new();

	public bool IsDead() => CurrentHP <= 0;

	public class Skill
	{
		public string Name;
		public int Cost;
		public SkillType Type;
		public TargetType Targeting;
		public int Power;
		public ElementType Element;
		public bool Unlocked;

		public void Activate(Character user, Character target)
		{
			if (user.CurrentSP < Cost)
			{
				GD.Print($"{user.CharacterName} doesn't have enough SP to use {Name}!");
				return;
			}
			user.CurrentSP = Math.Max(0, user.CurrentSP - Cost);
			GD.Print($"Skill {Name} activated by {user.CharacterName} on {target.CharacterName}");
			switch (Type)
			{
				case SkillType.Physical:
					int baseDmg = Math.Max(1, user.Strength + Power - target.Defense);
					user.DealDamage(target, baseDmg, Element);
					break;
				case SkillType.Magical:
					int magicDmg = Math.Max(1, user.Magic + Power - target.Defense);
					user.DealDamage(target, magicDmg, Element);
					break;
				case SkillType.Heal:
					target.Heal(Power + user.Magic);
					break;
				case SkillType.Buff:
					target.BuffStacks += 1;
					GD.Print($"{target.CharacterName} gained a buff! Total Buff Stacks: {target.BuffStacks}");
					break;
			}
		}
	}

	public void DealDamage(Character target, int baseDamage, ElementType element)
	{
		double finalDmg = baseDamage;

		if (target.Weaknesses.Contains(element))
		{
			finalDmg *= 1.5;
			GD.Print($"{target.CharacterName} is weak to {element}! Bonus damage!");
		}
		else if (target.Resistances.Contains(element))
		{
			finalDmg *= 0.5;
			GD.Print($"{target.CharacterName} resists {element}! Reduced damage!");
		}

		finalDmg *= 1 + (0.5 * BuffStacks);

		if (rng.NextDouble() <= 0.1)
		{
			finalDmg *= 2;
			GD.Print($"CRITICAL HIT!");
		}

		target.TakeDamage((int)Math.Round(finalDmg));
	}

	public void TakeDamage(int amount)
	{
		if (IsDefending)
		{
			amount = (int)Math.Ceiling(amount / 2.0);
			GD.Print($"{CharacterName} is defending! Blocked half the damage!");
		}
		CurrentHP = Math.Max(0, CurrentHP - amount);
		GD.Print($"{CharacterName} took {amount} dmg → {CurrentHP}/{MaxHP} HP");
		IsDefending = false;
	}

	public void Heal(int amount)
	{
		CurrentHP = Math.Min(MaxHP, CurrentHP + amount);
		GD.Print($"{CharacterName} healed {amount} → {CurrentHP}/{MaxHP} HP");
	}

	public void Defend()
	{
		IsDefending = true;
		GD.Print($"{CharacterName} is defending!");
	}
}
