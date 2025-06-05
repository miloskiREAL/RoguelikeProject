using Godot;
using System;
using System.Collections.Generic;

public partial class Character : Control
{
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
		switch (cls)
		{
			case CharacterClass.Knight:
				CharacterName = "Knight";
				MaxHP = 200; MaxSP = 50;
				Strength = 30; Magic = 5; Defense = 20; Speed = 10;
				Skills.Add(new Skill { Name="Slash", Power=20, Type=SkillType.Physical });
				Skills.Add(new Skill { Name="Defend", Power=0, Type=SkillType.Buff });
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
				Strength = 5;  Magic = 30; Defense = 8; Speed = 12;
				Skills.Add(new Skill { Name="Firebolt", Power=28, Element=ElementType.Fire, Type=SkillType.Magical });
				Skills.Add(new Skill { Name="Ice Shard", Power=26, Element=ElementType.Ice,  Type=SkillType.Magical });
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
		CurrentHP = Math.Max(0, CurrentHP - amount);
		GD.Print($"{CharacterName} took {amount} dmg → {CurrentHP}/{MaxHP} HP");
		
	}

	public void Heal(int amount)
	{
		CurrentHP = Math.Min(MaxHP, CurrentHP + amount);
		GD.Print($"{CharacterName} healed {amount} → {CurrentHP}/{MaxHP} HP");
	}

}
