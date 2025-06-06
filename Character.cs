using Godot;
using System;
using System.Collections.Generic;

public partial class Character : Control
{
	public CharacterClass Class { get; private set; }
	public enum SkillType { Physical, Magical, Heal, Buff, Debuff };
	public enum TargetType { SingleEnemy, AllEnemies, SingleAlly, AllAllies, Self };
	public enum ElementType { Neutral, Fire, Ice, Lightning, Holy, Wind, Earth };
	public enum CharacterClass { Knight, Monk, Wizard, Ranger };
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
	public List<Skill> Skills = new();
	public bool IsDead() => CurrentHP <= 0;
	public class Skill
	{
		public string Name;
		public int Cost;
		public SkillType Type; 
   		public int Power;
		public ElementType Element; 
		public bool Unlocked;    
		public void Activate(Character user, Character target)
		{
			 switch (Type)
			{
				case SkillType.Physical:
					int dmg = Math.Max(1, user.Strength + Power - target.Defense);
					target.TakeDamage(dmg);
					break;
				case SkillType.Magical:
					int mdmg = Math.Max(1, user.Magic + Power - target.Defense);
					target.TakeDamage(mdmg);
					break;
				case SkillType.Heal:
					target.Heal(Power + user.Magic);
					break;
		

		   
			}
		}
	}
	public void Init(CharacterClass cls)
	{
		Class = cls;
		switch (cls)
		{
			case CharacterClass.Knight:
				CharacterName = "Knight";
				MaxHP = 200 + SaveData.PartyLevel * 1.5; MaxSP = 100 + SaveData.PartyLevel * 1.5;
				Strength = 30 + SaveData.PartyLevel; 
				Magic = 5 + SaveData.PartyLevel; 
				Defense = 20 + SaveData.PartyLevel; 
				Speed = 10 + SaveData.PartyLevel;
				Skills.Add(new Skill { Name="Slash", Power=50, Type=SkillType.Physical, Targeting=TargetType.SingleEnemy, Element=ElementType.Neutral, Unlocked=True, cost=5 });
				Skills.Add(new Skill { Name="Motivate", Power=0, Type=SkillType.Buff, Targeting=TargetType.AllAllies, Element=ElementType.Neutral, Unlocked=True, cost=8 });
				Skills.Add(new Skill { Name="HolyBlade", Power=100, Type=SkillType.Physical, Targeting=TargetType.SingleEnemy, Element=ElementType.Holy, Unlocked=SaveData.PartyLevel>=5, cost=10 });
				Skills.Add(new Skill { Name="RoundSlash", Power=60, Type=SkillType.Physical, Targeting=TargetType.AllEnemies, Element=ElementType.Neutral, Unlocked=SaveData.PartyLevel>=10, cost=8 });
				Skills.Add(new Skill { Name="Cleave", Power=150, Type=SkillType.Physical, Targeting=TargetType.SingleEnemy, Element=ElementType.Neutral, Unlocked=SaveData.PartyLevel>=15, cost=20 });
				break;

			case CharacterClass.Monk:
				CharacterName = "Monk";
				MaxHP = 160; MaxSP = 60;
				Strength = 25; Magic = 10; Defense = 15; Speed = 18;
				Skills.Add(new Skill { Name="Flurry Fists", Power=18, Type=SkillType.Physical });
				Skills.Add(new Skill { Name="Meditate", Power=20, Type=SkillType.Heal });
				break;

			case CharacterClass.Wizard:
				CharacterName = "Wizard";
				MaxHP = 120; MaxSP = 120;
				Strength = 5  + SaveData.PartyLevel;  
				Magic = 30  + SaveData.PartyLevel; 
				Defense = 8  + SaveData.PartyLevel; 
				Speed = 12  + SaveData.PartyLevel;
				Skills.Add(new Skill { Name="Fireball", Power=40, Type=SkillType.Magical, Targeting=TargetType.SingleEnemy, Element=ElementType.Fire, Unlocked=True, cost=5 });
				Skills.Add(new Skill { Name="Iceshard", Power=40, Type=SkillType.Magical, Targeting=TargetType.SingleEnemy, Element=ElementType.Ice, Unlocked=True, cost=5 });
				Skills.Add(new Skill { Name="Heal", Power=60, Type=SkillType.Heal, Targeting=TargetType.SingleAlly, Element=ElementType.Neutral, Unlocked=SaveData.PartyLevel>=6, cost=7 });
				Skills.Add(new Skill { Name="Fireball", Power=40, Type=SkillType.Magical, Targeting=TargetType.SingleEnemy, Element=ElementType.Fire, Unlocked=True, cost=5 });
				break;

			case CharacterClass.Ranger:
				CharacterName = "Ranger";
				MaxHP = 140; MaxSP = 70;
				Strength = 22; Magic = 12; Defense = 12; Speed = 22;
				Skills.Add(new Skill { Name="Arrow Shot", Power=22, Type=SkillType.Physical });
				Skills.Add(new Skill { Name="Quick Step", Power=0,  Type=SkillType.Buff });
				break;
		}

		CurrentHP = MaxHP;
		CurrentSP = MaxSP;
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
