using Godot;
using System;
using System.Collections.Generic;

public partial class UIManager : Control
{
	[Export] private Control actionMenu;
	[Export] private Godot.Collections.Array<Control> skillMenus;
	[Export] private Control itemMenu;
	[Export] private VBoxContainer itemVBox;
	[Export] private Control targetMenu;
	[Export] private Button attackButton;
	[Export] private Button itemButton;
	[Export] private Button defendButton;
	[Export] private Button retreatButton;
	[Export] private Button skillBackButton;
	[Export] private Button targetBackButton;
	[Export] private Button itemBackButton; 

	private Character currentCharacter;
	private BattleManager battleManager;
	private Dictionary<Button, Action> skillButtonActions = new();
	private Dictionary<Button, Action> targetButtonActions = new(); 
	private Dictionary<Button, Action> itemButtonActions = new();
	private Item selectedItem;
	
	// Tracks what menu state you are in currently
	private enum MenuState
	{
		Action,
		Skill,
		Item,
		SkillTarget,
		ItemTarget
	}
	
	private MenuState currentMenuState = MenuState.Action;

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

	// Connects all buttons in the action menu to their respective functions
	private void ConnectActionButtons()
	{
		attackButton.Pressed += () => ShowSkillMenu(currentCharacter);
		itemButton.Pressed += () => ShowItemMenu(currentCharacter);
		defendButton.Pressed += () => battleManager.OnPlayerActionSelected("Defend");
		retreatButton.Pressed += () => battleManager.OnPlayerActionSelected("Retreat");
	}

	// Shows the main action menu buttons for the current character
	public void ShowActionButtons(Character character)
	{
		HideAllMenus();
		currentCharacter = character;
		actionMenu.Visible = true;
		currentMenuState = MenuState.Action;
	}

	// Shows all unlocked skills for the current character and disables the button if that character doesn't have enough SP
	public void ShowSkillMenu(Character character)
	{
		HideAllMenus();
		currentCharacter = character;
		skillBackButton.Visible = true;
		currentMenuState = MenuState.Skill;

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

	// Shows all item buttons, disables them if there are none in inventory, and displays the count of how many you have
	public void ShowItemMenu(Character character)
	{
		HideAllMenus();
		currentCharacter = character;
		itemMenu.Visible = true;
		itemBackButton.Visible = true;
		currentMenuState = MenuState.Item;

		// Clear previous button presses
		foreach (var entry in itemButtonActions)
		{
			entry.Key.Pressed -= entry.Value;
		}
		itemButtonActions.Clear();

		foreach (Button btn in itemVBox.GetChildren())
		{
			if (Item.All.TryGetValue(btn.Name, out Item item))
			{
				btn.Visible = true;
				int count = 0;
				GameManager.Instance.SaveData.Inventory.TryGetValue(item.Key, out count);
				btn.Text = $"{item.Name} x{count}";
				
				if (count > 0)
				{
					btn.Disabled = false;
					Action handler = () => OnItemButtonPressed(item);
					btn.Pressed += handler;
					itemButtonActions[btn] = handler;
				}
				else
				{
					btn.Disabled = true;
				}
			}
			else
			{
				btn.Visible = true;
				btn.Disabled = true;
				btn.Text = $"{btn.Name} x0";
			}
		}
	}

	// Sends you to targeting menu if single, and just uses it if party wide
	private void OnItemButtonPressed(Item item)
	{
		if (item == null)
		{
			GD.PrintErr("OnItemButtonPressed: Item is null!");
			return;
		}

		GD.Print($"OnItemButtonPressed: {item.Name}, IsTeamWide: {item.IsTeamWide}");
		selectedItem = item;
		
		if (item.IsTeamWide)
		{
			// For teamwide items, call battleManager directly with null target
			battleManager.OnItemSelected(item, null);
		}
		else
		{
			// For single target items, show target selection menu
			ShowItemTargetMenu(item);
		}
	}

	// Shows the target selection menu for items that target a single ally
	private void ShowItemTargetMenu(Item item)
	{
		if (item == null)
		{
			GD.PrintErr("ShowItemTargetMenu: Item is null!");
			return;
		}

		GD.Print($"ShowItemTargetMenu: Setting up targets for {item.Name}");
		
		HideAllMenus();
		targetMenu.Visible = true;
		targetBackButton.Visible = true;
		currentMenuState = MenuState.ItemTarget; 
		selectedItem = item;

		// Clear previous target button handlers
		foreach (Button btn in targetMenu.GetChildren())
		{
			if (targetButtonActions.TryGetValue(btn, out var oldHandler))
				btn.Pressed -= oldHandler;
			btn.Visible = false;
		}
		targetButtonActions.Clear();

		// Set up ally target buttons for single-target items
		for (int i = 0; i < battleManager.playerParty.Count; i++)
		{
			var ally = battleManager.playerParty[i];
			if (ally.IsDead()) continue;

			var btn = targetMenu.GetNode<Button>($"TargetAlly{i + 1}");
			btn.Visible = true;

			var label = btn.GetNode<Label>("ClassLabel");
			label.Text = ally.CharacterName; 

			Item capturedItem = item; 
			Character capturedAlly = ally;
			
			Action handler = () => {
				GD.Print($"Target selected: {capturedItem.Name} on {capturedAlly.CharacterName}");
				battleManager.OnItemSelected(capturedItem, capturedAlly);
			};
			
			btn.Pressed += handler;
			targetButtonActions[btn] = handler;
		}
	}

	// Shows the target menu based on the skill's targeting type
	public void ShowTargetMenu(Character.Skill skill)
	{
		HideAllMenus();
		targetMenu.Visible = true;
		targetBackButton.Visible = true;
		currentMenuState = MenuState.SkillTarget;

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

	// Connects all back buttons to their respective handlers
	private void ConnectBackButtons()
	{
		skillBackButton.Visible = false;
		targetBackButton.Visible = false;
		itemBackButton.Visible = false;

		skillBackButton.Pressed -= OnSkillBackPressed;
		targetBackButton.Pressed -= OnTargetBackPressed;
		itemBackButton.Pressed -= OnItemBackPressed;

		skillBackButton.Pressed += OnSkillBackPressed;
		targetBackButton.Pressed += OnTargetBackPressed;
		itemBackButton.Pressed += OnItemBackPressed;
	}

	// Handles going back from skill menu to action menu
	private void OnSkillBackPressed()
	{
		HideAllMenus();
		ShowActionButtons(currentCharacter);
	}

	// Handles going back from target menu to either skill or item menu based on current state
	private void OnTargetBackPressed()
	{
		// Use menu state instead of selectedItem to determine where to go back
		switch (currentMenuState)
		{
			case MenuState.ItemTarget:
				ShowItemMenu(currentCharacter);
				break;
			case MenuState.SkillTarget:
				ShowSkillMenu(currentCharacter);
				break;
			default:
				// Fallback to action menu if state is unclear
				ShowActionButtons(currentCharacter);
				break;
		}
	}

	// Handles going back from item menu to action menu
	private void OnItemBackPressed()
	{
		selectedItem = null;
		HideAllMenus();
		ShowActionButtons(currentCharacter);
	}

	// Sets visibility of all menus and buttons to false 
	// Also disconnects all button presses to prevent any bugs
	public void HideAllMenus()
	{
		actionMenu.Visible = false;
		itemMenu.Visible = false;
		targetMenu.Visible = false;
		skillBackButton.Visible = false;
		targetBackButton.Visible = false;
		itemBackButton.Visible = false;

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
		
		foreach (var entry in itemButtonActions)
		{
			entry.Key.Pressed -= entry.Value;
		}
		itemButtonActions.Clear();
	}
}
