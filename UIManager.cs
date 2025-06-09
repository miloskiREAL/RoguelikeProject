using Godot;
using System;
using System.Collections.Generic;

public partial class UIManager : Control
{
	[Export] private Control actionMenu;
	[Export] private Godot.Collections.Array<Control> skillMenus;
	[Export] private Control itemMenu;
	[Export] private Control targetMenu;
	[Export] private Button attackButton;
	[Export] private Button skillButton;
	[Export] private Button itemButton;
	[Export] private Button defendButton;
	[Export] private Button retreatButton;

	private Character currentCharacter;
	private BattleManager battleManager;

	public override void _Ready()
	{
		HideAllMenus();
		ConnectActionButtons();
	}

	public void SetBattleManager(BattleManager bm)
	{
		battleManager = bm;
	}

	private void ConnectActionButtons()
	{
		attackButton.Pressed += () => battleManager.OnPlayerActionSelected("Attack");
		skillButton.Pressed += () => battleManager.OnPlayerActionSelected("Skill");
		itemButton.Pressed += () => battleManager.OnPlayerActionSelected("Item");
		defendButton.Pressed += () => battleManager.OnPlayerActionSelected("Defend");
		retreatButton.Pressed += () => battleManager.OnPlayerActionSelected("Retreat");
	}

	public void ShowActionButtons(Character character)
	{
		HideAllMenus();
		currentCharacter = character;
		actionMenu.Visible = true;
	}

	public void ShowSkillMenu(Character character)
	{
		HideAllMenus();
		currentCharacter = character;

		foreach (var menu in skillMenus)
		{
			bool isTargetMenu = menu.Name == $"{character.CharacterName}SkillMenu";
			menu.Visible = isTargetMenu;

			if (!isTargetMenu) continue;

			foreach (Button btn in menu.GetChildren())
			{
				var skill = character.Skills.Find(s => s.Name == btn.Name);
				if (skill != null)
				{
					btn.Visible = skill.Unlocked;
					btn.Disabled = character.CurrentSP < skill.Cost;
					btn.Pressed += () => battleManager.OnSkillSelected(skill);
				}
				else
				{
					btn.Visible = false;
					btn.Disabled = true;
				}
			}
		}
	}

	public void ShowItemMenu(Character character)
	{
		HideAllMenus();
		currentCharacter = character;
		itemMenu.Visible = true;
		// Item logic here if needed
	}

	public void ShowTargetMenu(Character.Skill skill)
	{
		HideAllMenus();
		targetMenu.Visible = true;
		// Add targeting logic based on skill.TargetType if needed
	}

	public void HideAllMenus()
	{
		actionMenu.Visible = false;
		itemMenu.Visible = false;
		targetMenu.Visible = false;

		foreach (var menu in skillMenus)
		{
			menu.Visible = false;
			foreach (Button btn in menu.GetChildren())
			{
				btn.Pressed -= null; // Disconnect all previous signals (symbolic cleanup, needs actual disconnects in real use)
			}
		}
	}
}
