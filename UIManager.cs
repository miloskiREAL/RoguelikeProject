using Godot;
using System;
using System.Collections.Generic;

public partial class UIManager : Control
{
	public static UIManager Instance;

	[Export] private Control actionMenu;
	[Export] private List<Control> skillMenus; // Each index matches a character's skill menu (e.g., Knight = 0, Mage = 1...)
	[Export] private Control itemMenu;
	[Export] private Control targetMenu;

	public override void _Ready()
	{
		Instance = this;
		HideAllMenus();
	}

	public void ShowActionButtons(Character character)
	{
		HideAllMenus();
		actionMenu.Visible = true;
	}

	public void ShowSkillMenu(Character character)
	{
		HideAllMenus();

		// Identify correct skill menu by character name or enum
		foreach (var menu in skillMenus)
		{
			menu.Visible = menu.Name == $"{character.CharacterName}SkillMenu";

			// Hide locked skills if needed
			foreach (Button btn in menu.GetChildren())
			{
				var skill = character.Skills.Find(s => s.Name == btn.Name);
				if (skill != null)
					btn.Visible = skill.Unlocked;
				else
					btn.Visible = false;
			}
		}
	}

	public void ShowItemMenu(Character character)
	{
		HideAllMenus();
		itemMenu.Visible = true;

		// You could update shared item list here if needed
	}

	public void ShowTargetMenu(Character.Skill skill)
	{
		HideAllMenus();
		targetMenu.Visible = true;

		// Highlight valid targets based on skill.TargetType
		// e.g., highlight enemy buttons or ally buttons
		// This logic would go here if needed
	}

	public void HideAllMenus()
	{
		actionMenu.Visible = false;
		itemMenu.Visible = false;
		targetMenu.Visible = false;

		foreach (var menu in skillMenus)
			menu.Visible = false;
	}
}
