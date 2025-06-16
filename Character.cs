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
	public int CriticalChance = 10;
	public int DamageReduction = 0;
	public bool IsDefending = false;
	public int BuffStacks = 0;
	public int DebuffStacks = 0;
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
		// Defines what happens when each skill type is activated
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
				case SkillType.Debuff:
					target.DebuffStacks += 1;
					target.activityIndicator?.AddMessage($"{target.CharacterName} is debuffed! (Total: {target.DebuffStacks})");
					break;
			}
		}
	}
	// This is how damage is calculated
	public void DealDamage(Character target, int baseDamage, ElementType element)
	{
		double finalDmg = baseDamage;

		// Element weakness/resistance
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

		// Buff stacks (attacker damage bonus)
		finalDmg *= 1 + (0.5 * BuffStacks);

		// Debuff stacks (target takes more damage)
		if (target.DebuffStacks > 0)
		{
			double debuffMultiplier = 1 + (0.3 * target.DebuffStacks);
			finalDmg *= debuffMultiplier;
			activityIndicator?.AddMessage($"{target.CharacterName} takes {(debuffMultiplier - 1) * 100:F0}% more damage due to debuffs!");
		}

		// Critical hit check using CriticalChance
		bool isCritical = false;
		if (rng.Next(1, 101) <= CriticalChance) // 1-100 roll vs your critical chance
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
	// How damage taken is calculated
	public void TakeDamage(int amount)
	{
		double defenseReduction = Defense * 0.001;
		defenseReduction = Math.Min(defenseReduction, 0.5); 
	
		if (defenseReduction > 0)
		{
			double defenseMultiplier = 1.0 - defenseReduction;
			amount = (int)Math.Ceiling(amount * defenseMultiplier);
		}

		if (DamageReduction > 0)
		{
			double reductionMultiplier = 1.0 - (DamageReduction / 100.0);
			amount = (int)Math.Ceiling(amount * reductionMultiplier);
			activityIndicator?.AddMessage($"{CharacterName} reduces damage by {DamageReduction}%!");
		}

		if (IsDefending)
		{
			amount = (int)Math.Ceiling(amount / 2.0);
			activityIndicator?.AddMessage($"{CharacterName} blocks half the damage!");
		}

		CurrentHP = Math.Max(0, CurrentHP - amount);
		activityIndicator?.Log($"{CharacterName} → {CurrentHP}/{MaxHP} HP");
		IsDefending = false;
		if (DebuffStacks > 0)
		{
			int clearedStacks = DebuffStacks;
			DebuffStacks = 0;
			activityIndicator?.AddMessage($"{CharacterName}'s {clearedStacks} debuff stack(s) are cleared after taking damage!");
		}
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
	// Sets Defending to be true and halves incoming damage
	public void Defend()
	{
		IsDefending = true;
		activityIndicator?.AddMessage($"{CharacterName} takes a defensive stance");
	}
	// Updates the health and sp bars 
	public void UpdateUI()
	{
		var hpBar = GetNodeOrNull<ProgressBar>("HPBar");
		var spBar = GetNodeOrNull<ProgressBar>("SPBar");
		var hpLabel = GetNodeOrNull<Label>("HPBar/HPLabel");
		var spLabel = GetNodeOrNull<Label>("SPBar/SPLabel");
	
		if (hpBar != null)
		{
			hpBar.MaxValue = MaxHP;
			hpBar.Value = CurrentHP;
		}
		
		if (spBar != null)
		{
			spBar.MaxValue = MaxSP;
			spBar.Value = CurrentSP;
		}
		
		if (hpLabel != null)
		{
			hpLabel.Text = $"{CurrentHP}/{MaxHP}";
		}
		if (spLabel != null)
		{
			spLabel.Text = $"{CurrentSP}/{MaxSP}";
		}

		// Only animate if we have the bars
		if (hpBar != null && spBar != null)
		{
			var tween = CreateTween();
			tween.TweenProperty(hpBar, "value", CurrentHP, 0.3f);
			tween.TweenProperty(spBar, "value", CurrentSP, 0.3f);
		}
	}
}
