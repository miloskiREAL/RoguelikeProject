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
	[Export] private Button itemButton;
	[Export] private Button defendButton;
	[Export] private Button retreatButton;

	private Character currentCharacter;
	private BattleManager battleManager;

	private Dictionary<Button, Action> skillButtonActions = new();

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
		attackButton.Pressed += () => ShowSkillMenu(currentCharacter);
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
			bool isCorrectMenu = menu.Name == $"{character.CharacterName}SkillMenu";
			menu.Visible = isCorrectMenu;

			if (!isCorrectMenu) continue;

			foreach (Button btn in menu.GetChildren())
			{
				if (skillButtonActions.TryGetValue(btn, out var oldHandler))
					btn.Pressed -= oldHandler;

				var skill = character.Skills.Find(s => s.Name == btn.Name);
				if (skill != null)
				{
					btn.Visible = skill.Unlocked;
					btn.Disabled = character.CurrentSP < skill.Cost;

					Action handler = () => battleManager.OnSkillSelected(skill);
					btn.Pressed += handler;
					skillButtonActions[btn] = handler;
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
	}

	public void ShowTargetMenu(Character.Skill skill)
	{
		HideAllMenus();
		targetMenu.Visible = true;
		// Add targeting button logic here
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
				if (skillButtonActions.TryGetValue(btn, out var handler))
					btn.Pressed -= handler;
			}
		}
		skillButtonActions.Clear();
	}
}
