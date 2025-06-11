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
	[Export] private Button skillBackButton;
	[Export] private Button targetBackButton;

	private Character currentCharacter;
	private BattleManager battleManager;
	private Dictionary<Button, Action> skillButtonActions = new();
	private Dictionary<Button, Action> targetButtonActions = new(); 

	public override void _Ready()
	{
		HideAllMenus();
		ConnectActionButtons();
		ConnectBackButtons();
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
		skillBackButton.Visible = true;
		
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
		targetBackButton.Visible = true;

		foreach (Button btn in targetMenu.GetChildren())
		{
			if (targetButtonActions.TryGetValue(btn, out var oldHandler))
				btn.Pressed -= oldHandler;
			btn.Visible = false;
		}
		targetButtonActions.Clear();

		switch (skill.Targeting)
		{
			case Character.TargetType.SingleEnemy:
				for (int i = 0; i < battleManager.enemyParty.Count; i++)
				{
					var enemy = battleManager.enemyParty[i];
					if (enemy.IsDead()) continue;

					var btn = targetMenu.GetNode<Button>($"TargetEnemy{i + 1}");
					btn.Visible = true;

					var label = btn.GetNode<Label>("ClassLabel");
					label.Text = enemy.Class.ToString();

					Action handler = () => battleManager.OnTargetSelected(enemy, skill);
					btn.Pressed += handler;
					targetButtonActions[btn] = handler;
				}
				break;

			case Character.TargetType.AllEnemies:
				var allEnemiesBtn = targetMenu.GetNode<Button>("TargetAllEnemies");
				allEnemiesBtn.Visible = true;

				var allEnemiesLabel = allEnemiesBtn.GetNode<Label>("ClassLabel");
				allEnemiesLabel.Text = "All Enemies";

				Action allEnemiesHandler = () => battleManager.OnTargetSelected(null, skill); 
				allEnemiesBtn.Pressed += allEnemiesHandler;
				targetButtonActions[allEnemiesBtn] = allEnemiesHandler;
				break;

			case Character.TargetType.SingleAlly:
				for (int i = 0; i < battleManager.playerParty.Count; i++)
				{
					var ally = battleManager.playerParty[i];
					if (ally.IsDead()) continue;

					var btn = targetMenu.GetNode<Button>($"TargetAlly{i + 1}");
					btn.Visible = true;

					var label = btn.GetNode<Label>("ClassLabel");
					label.Text = ally.Class.ToString();

					Action handler = () => battleManager.OnTargetSelected(ally, skill);
					btn.Pressed += handler;
					targetButtonActions[btn] = handler;
				}
				break;

			case Character.TargetType.AllAllies:
				var allAlliesBtn = targetMenu.GetNode<Button>("TargetAllAllies");
				allAlliesBtn.Visible = true;

				var allAlliesLabel = allAlliesBtn.GetNode<Label>("ClassLabel");
				allAlliesLabel.Text = "All Allies";

				Action allAlliesHandler = () => battleManager.OnTargetSelected(null, skill); 
				allAlliesBtn.Pressed += allAlliesHandler;
				targetButtonActions[allAlliesBtn] = allAlliesHandler;
				break;
		}
	}

	private void ConnectBackButtons()
	{
		skillBackButton.Visible = false;
		targetBackButton.Visible = false;

		skillBackButton.Pressed -= OnSkillBackPressed;
		targetBackButton.Pressed -= OnTargetBackPressed;

		skillBackButton.Pressed += OnSkillBackPressed;
		targetBackButton.Pressed += OnTargetBackPressed;
	}

	private void OnSkillBackPressed()
	{
		HideAllMenus();
		ShowActionButtons(currentCharacter);
	}

	private void OnTargetBackPressed()
	{
		HideAllMenus();
		ShowSkillMenu(currentCharacter);
	}

	public void HideAllMenus()
	{
		actionMenu.Visible = false;
		itemMenu.Visible = false;
		targetMenu.Visible = false;
		skillBackButton.Visible = false;
		targetBackButton.Visible = false;

		
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

		
		foreach (Button btn in targetMenu.GetChildren())
		{
			if (targetButtonActions.TryGetValue(btn, out var handler))
				btn.Pressed -= handler;
		}
		targetButtonActions.Clear();
	}
}
