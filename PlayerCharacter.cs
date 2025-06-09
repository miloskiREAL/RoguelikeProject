using Godot;
using System;

public partial class PlayerCharacter : Character
{
	public enum CharacterClass { Knight, Monk, Wizard, Ranger }

	[Export]
	public CharacterClass Class;

	public void Init(CharacterClass cls)
	{
		Class = cls;
		int partyLevel = GameManager.Instance?.SaveData?.PartyLevel ?? 1;

		Weaknesses.Clear();
		Resistances.Clear();
		Skills.Clear();

		switch (cls)
		{
			case CharacterClass.Knight:
				CharacterName = "Knight";
				MaxHP = 200 + (int)(partyLevel * 1.5);
				MaxSP = 100 + (int)(partyLevel * 1.5);
				Strength = 30 + partyLevel;
				Magic = 5 + partyLevel;
				Defense = 20 + partyLevel;
				Speed = 10 + partyLevel;

				Weaknesses.Add(ElementType.Fire);
				Weaknesses.Add(ElementType.Darkness);

				Resistances.Add(ElementType.Holy);
				Resistances.Add(ElementType.Neutral);

				Skills.Add(new Skill { Name = "Slash", Power = 50, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Unlocked = true, Cost = 5 });
				Skills.Add(new Skill { Name = "Motivate", Power = 0, Type = SkillType.Buff, Targeting = TargetType.AllAllies, Element = ElementType.Neutral, Unlocked = true, Cost = 8 });
				Skills.Add(new Skill { Name = "HolyBlade", Power = 100, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Holy, Unlocked = partyLevel >= 5, Cost = 10 });
				Skills.Add(new Skill { Name = "RoundSlash", Power = 60, Type = SkillType.Physical, Targeting = TargetType.AllEnemies, Element = ElementType.Neutral, Unlocked = partyLevel >= 10, Cost = 8 });
				Skills.Add(new Skill { Name = "Cleave", Power = 200, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Unlocked = partyLevel >= 15, Cost = 20 });
				break;

			case CharacterClass.Monk:
				CharacterName = "Monk";
				MaxHP = 160 + (int)(partyLevel * 1.4);
				MaxSP = 60 + (int)(partyLevel * 1.2);
				Strength = 25 + partyLevel;
				Magic = 10 + (int)(partyLevel * 0.7);
				Defense = 15 + (int)(partyLevel * 0.9);
				Speed = 18 + partyLevel;

				Weaknesses.Add(ElementType.Ice);
				Weaknesses.Add(ElementType.Wind);

				Resistances.Add(ElementType.Earth);
				Resistances.Add(ElementType.Lightning);

				Skills.Add(new Skill { Name = "Punch", Power = 60, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Unlocked = true, Cost = 5 });
				Skills.Add(new Skill { Name = "Rocksling", Power = 60, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Earth, Unlocked = true, Cost = 5 });
				Skills.Add(new Skill { Name = "Meditate", Power = 0, Type = SkillType.Buff, Targeting = TargetType.SingleAlly, Element = ElementType.Neutral, Unlocked = partyLevel >= 7, Cost = 8 });
				Skills.Add(new Skill { Name = "FlurryBlows", Power = 70, Type = SkillType.Physical, Targeting = TargetType.AllEnemies, Element = ElementType.Neutral, Unlocked = partyLevel >= 12, Cost = 8 });
				Skills.Add(new Skill { Name = "EarthShatter", Power = 125, Type = SkillType.Physical, Targeting = TargetType.AllEnemies, Element = ElementType.Earth, Unlocked = partyLevel >= 18, Cost = 25 });
				break;

			case CharacterClass.Wizard:
				CharacterName = "Wizard";
				MaxHP = 120 + (int)(partyLevel * 1.2);
				MaxSP = 120 + (int)(partyLevel * 1.8);
				Strength = 5 + partyLevel;
				Magic = 30 + (int)(partyLevel * 1.5);
				Defense = 8 + partyLevel;
				Speed = 12 + partyLevel;

				Weaknesses.Add(ElementType.Neutral);
				Weaknesses.Add(ElementType.Holy);

				Resistances.Add(ElementType.Fire);
				Resistances.Add(ElementType.Ice);

				Skills.Add(new Skill { Name = "Fireball", Power = 40, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Fire, Unlocked = true, Cost = 5 });
				Skills.Add(new Skill { Name = "Iceshard", Power = 40, Type = SkillType.Magical, Targeting = TargetType.SingleEnemy, Element = ElementType.Ice, Unlocked = true, Cost = 5 });
				Skills.Add(new Skill { Name = "Heal", Power = 60, Type = SkillType.Heal, Targeting = TargetType.SingleAlly, Element = ElementType.Neutral, Unlocked = partyLevel >= 6, Cost = 7 });
				Skills.Add(new Skill { Name = "Meteor", Power = 100, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Fire, Unlocked = partyLevel >= 16, Cost = 30 });
				Skills.Add(new Skill { Name = "IceAge", Power = 100, Type = SkillType.Magical, Targeting = TargetType.AllEnemies, Element = ElementType.Ice, Unlocked = partyLevel >= 16, Cost = 30 });
				break;

			case CharacterClass.Ranger:
				CharacterName = "Ranger";
				MaxHP = 140 + (int)(partyLevel * 1.3);
				MaxSP = 70 + (int)(partyLevel * 1.4);
				Strength = 22 + (int)(partyLevel * 1.5);
				Magic = 12 + (int)(partyLevel * 0.8);
				Defense = 12 + (int)(partyLevel * 0.8);
				Speed = 22 + partyLevel;

				Weaknesses.Add(ElementType.Lightning);
				Weaknesses.Add(ElementType.Earth);

				Resistances.Add(ElementType.Wind);
				Resistances.Add(ElementType.Darkness);

				Skills.Add(new Skill { Name = "Arrowshot", Power = 70, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Unlocked = true, Cost = 6 });
				Skills.Add(new Skill { Name = "WindShot", Power = 70, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Wind, Unlocked = true, Cost = 6 });
				Skills.Add(new Skill { Name = "ArrowRain", Power = 90, Type = SkillType.Physical, Targeting = TargetType.AllEnemies, Element = ElementType.Neutral, Unlocked = partyLevel >= 8, Cost = 9 });
				Skills.Add(new Skill { Name = "HurricaneArrow", Power = 120, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Wind, Unlocked = partyLevel >= 13, Cost = 10 });
				Skills.Add(new Skill { Name = "OneShot", Power = 300, Type = SkillType.Physical, Targeting = TargetType.SingleEnemy, Element = ElementType.Neutral, Unlocked = partyLevel >= 20, Cost = 25 });
				break;
		}

		CurrentHP = MaxHP;
		CurrentSP = MaxSP;
	}
}
