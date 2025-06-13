using Godot;
using System;
using System.Collections.Generic;

public partial class Character : Node2D
{
	public enum SkillType { Physical, Magical, Heal, Buff, Debuff }
	public enum TargetType { SingleEnemy, AllEnemies, SingleAlly, AllAllies }
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
	protected ActivityIndicator activityIndicator;

	public bool IsDead() => CurrentHP <= 0;

	public void SetActivityIndicator(ActivityIndicator indicator)
	{
		activityIndicator = indicator;
	}

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
				user.activityIndicator?.AddMessage($"{user.CharacterName} doesn't have enough SP for {Name}!");
				return;
			}
			user.CurrentSP = Math.Max(0, user.CurrentSP - Cost);
			user.UpdateUI();
			user.activityIndicator?.AddMessage($"{user.CharacterName} uses {Name}!");
			
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
					target.activityIndicator?.AddMessage($"{target.CharacterName} gains a buff! (Total: {target.BuffStacks})");
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
			activityIndicator?.AddMessage($"{target.CharacterName} is weak to {element}! Extra damage!");
		}
		else if (target.Resistances.Contains(element))
		{
			finalDmg *= 0.5;
			activityIndicator?.AddMessage($"{target.CharacterName} resists {element}! Reduced damage!");
		}

		finalDmg *= 1 + (0.5 * BuffStacks);

		bool isCritical = false;
		if (rng.NextDouble() <= 0.1)
		{
			finalDmg *= 2;
			isCritical = true;
			activityIndicator?.AddMessage("CRITICAL HIT!");
		}

		int finalDamage = (int)Math.Round(finalDmg);
		target.TakeDamage(finalDamage);
		
		string damageMsg = isCritical 
			? $"CRITICAL! {CharacterName} deals {finalDamage} damage to {target.CharacterName}!"
			: $"{CharacterName} deals {finalDamage} damage to {target.CharacterName}";
		activityIndicator?.AddMessage(damageMsg);
	}

	public void TakeDamage(int amount)
	{
		if (IsDefending)
		{
			amount = (int)Math.Ceiling(amount / 2.0);
			activityIndicator?.AddMessage($"{CharacterName} blocks half the damage!");
		}
		CurrentHP = Math.Max(0, CurrentHP - amount);
		activityIndicator?.Log($"{CharacterName} → {CurrentHP}/{MaxHP} HP");
		IsDefending = false;
		
		if (IsDead())
		{
			activityIndicator?.AddMessage($"{CharacterName} has been defeated!");
		}
		
		UpdateUI();
	}

	public void Heal(int amount)
	{
		int actualHeal = Math.Min(amount, MaxHP - CurrentHP);
		CurrentHP = Math.Min(MaxHP, CurrentHP + amount);
		activityIndicator?.AddMessage($"{CharacterName} recovers {actualHeal} HP");
		UpdateUI();
	}

	public void Defend()
	{
		IsDefending = true;
		activityIndicator?.AddMessage($"{CharacterName} takes a defensive stance");
	}

	public void UpdateUI()
	{
		var hpBar = GetNode<ProgressBar>("HPBar");
		var spBar = GetNode<ProgressBar>("SPBar");
		var hpLabel = GetNodeOrNull<Label>("HPBar/HPLabel");
		var spLabel = GetNodeOrNull<Label>("SPBar/SPLabel");
	
		hpBar.MaxValue = MaxHP;
		hpBar.Value = CurrentHP;
		spBar.MaxValue = MaxSP;
		spBar.Value = CurrentSP;
		
		if (hpLabel != null)
		{
			hpLabel.Text = $"{CurrentHP}/{MaxHP}";
		}
		if (spLabel != null)
		{
			spLabel.Text = $"{CurrentSP}/{MaxSP}";
		}

		var tween = CreateTween();
		tween.TweenProperty(hpBar, "value", CurrentHP, 0.3f);
		tween.TweenProperty(spBar, "value", CurrentSP, 0.3f);
	}
}
