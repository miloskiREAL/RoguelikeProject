using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BattleManager : Control
{
	public List<PlayerCharacter> playerParty = new();
	public List<Enemy> enemyParty = new(); 
	private int turnIndex = 0;
	private bool isPlayerTurn = true;

	private Character.Skill selectedSkill = null;

	[Export] private NodePath playerPartyContainerPath;
	[Export] private NodePath enemyPartyContainerPath;
	[Export] private NodePath uiManagerPath;
	[Export] private NodePath activityIndicatorPath;

	private Control playerPartyContainer;
	private Control enemyPartyContainer;
	private UIManager uiManager;
	private ActivityIndicator activityIndicator;

	// Initialize the battle system and load parties
	public override void _Ready()
	{
		playerPartyContainer = GetNode<Control>(playerPartyContainerPath);
		enemyPartyContainer = GetNode<Control>(enemyPartyContainerPath);
		uiManager = GetNode<UIManager>(uiManagerPath);
		activityIndicator = GetNode<ActivityIndicator>(activityIndicatorPath);

		uiManager.SetBattleManager(this); 

		LoadPlayerParty();
		LoadEnemyParty();
		StartBattle();
	}

	// Load player characters from the scene and initialize them
	private void LoadPlayerParty()
	{
		var children = playerPartyContainer.GetChildren();
		children.Reverse();
		foreach (PlayerCharacter character in children)
		{
			playerParty.Add(character);
			character.SetActivityIndicator(activityIndicator);
			character.Init(character.Class);
			character.UpdateUI(); 
		}
	}

	// Load enemy characters from the scene and initialize them
	private void LoadEnemyParty()
	{
		foreach (Enemy enemy in enemyPartyContainer.GetChildren())
		{
			enemyParty.Add(enemy);
			enemy.SetActivityIndicator(activityIndicator);
			enemy.Init(enemy.Class);
			enemy.UpdateUI(); 
		}
	}

	// Begin the battle and start the first turn
	private void StartBattle()
	{
		turnIndex = 0;
		isPlayerTurn = true;
		activityIndicator.AddMessage("Battle begins!");
		StartTurn();
	}

	// Main turn logic, checks win conditions and manages turn flow
	private void StartTurn()
	{
		// Check for battle end conditions
		if (playerParty.All(p => p.IsDead()))
		{
			EndBattle(BattleResult.Defeat);
			return;
		}
		
		if (enemyParty.All(e => e.IsDead()))
		{
			EndBattle(BattleResult.Victory);
			return;
		}

		if (isPlayerTurn)
		{			
			// Move to enemy turn if all players have acted
			if (turnIndex >= playerParty.Count)
			{
				isPlayerTurn = false;
				turnIndex = 0;
				StartTurn();
				return;
			}

			var currentChar = playerParty[turnIndex];
			// Skip dead characters
			if (currentChar.IsDead())
			{
				turnIndex++;
				StartTurn();
				return;
			}

			selectedSkill = null;
			activityIndicator.AddMessage($"{currentChar.CharacterName}'s turn");
			uiManager.ShowActionButtons(currentChar);
		}
		else
		{
			EnemyTurn();
		}
	}

	// Handle player action selection (Skill, Item, Defend, Retreat)
	public void OnPlayerActionSelected(string actionType)
	{
		var currentChar = playerParty[turnIndex];

		switch (actionType)
		{
			case "Skill":
				uiManager.ShowSkillMenu(currentChar);
				break;
			case "Item":
				uiManager.ShowItemMenu(currentChar);
				break;
			case "Defend":
				currentChar.Defend();
				EndPlayerTurn();
				break;
			case "Retreat":
				bool success = TryRetreat(currentChar);
				if (success)
				{
					activityIndicator.AddMessage($"{currentChar.CharacterName} successfully retreats!");
					EndBattle(BattleResult.Retreat);
				}
				else
				{
					activityIndicator.AddMessage($"{currentChar.CharacterName} couldn't escape!");
					EndPlayerTurn();
				}
				break;
		}
	}

	// Store selected skill and show target selection menu
	public void OnSkillSelected(Character.Skill skill)
	{
		selectedSkill = skill;
		uiManager.ShowTargetMenu(skill);
	}

	// Handle item usage on characters
	public void OnItemSelected(Item item, Character target)
	{
		// Check if we have the item in inventory
		if (!GameManager.Instance.SaveData.Inventory.ContainsKey(item.Key) || 
			GameManager.Instance.SaveData.Inventory[item.Key] <= 0)
		{
			activityIndicator.AddMessage($"No {item.Name} available!");
			return;
		}

		bool itemUsed = false;
		
		// Handle teamwide items
		if (item.IsTeamWide)
		{
			var aliveAllies = playerParty.Where(p => !p.IsDead()).Cast<Character>().ToList();
			if (aliveAllies.Count > 0)
			{
				if (item.MultiTargetEffect != null)
				{
					item.MultiTargetEffect.Invoke(aliveAllies);
				}
				else if (item.SingleTargetEffect != null)
				{
					foreach (var ally in aliveAllies)
					{
						item.SingleTargetEffect.Invoke(ally);
					}
				}
				itemUsed = true;
				activityIndicator.AddMessage($"Used {item.Name} on all party members");
			}
		}
		// Handle single target items
		else
		{
			if (target != null && !target.IsDead() && item.SingleTargetEffect != null)
			{
				item.SingleTargetEffect.Invoke(target);
				itemUsed = true;
				activityIndicator.AddMessage($"Used {item.Name} on {target.CharacterName}");
			}
		}

		// Remove item from inventory and update UI
		if (itemUsed)
		{
			GameManager.Instance.SaveData.Inventory[item.Key]--;
			if (GameManager.Instance.SaveData.Inventory[item.Key] <= 0)
			{
				GameManager.Instance.SaveData.Inventory.Remove(item.Key);
			}
			
			foreach (var character in playerParty)
			{
				character.UpdateUI();
			}

			EndPlayerTurn();
		}
	}

	// Execute selected skill on target/s
	public void OnTargetSelected(Character target, Character.Skill skill)
	{
		var user = playerParty[turnIndex];
		
		if (selectedSkill == null) return;

		// Handle different targeting types
		switch (selectedSkill.Targeting)
		{
			case Character.TargetType.SingleEnemy:
			case Character.TargetType.SingleAlly:
				if (target != null)
				{
					selectedSkill.Activate(user, target);
				}
				break;

			case Character.TargetType.AllEnemies:
				foreach (var enemy in enemyParty)
				{
					if (!enemy.IsDead())
						selectedSkill.Activate(user, enemy);
				}
				break;

			case Character.TargetType.AllAllies:
				foreach (var ally in playerParty)
				{
					if (!ally.IsDead())
						selectedSkill.Activate(user, ally);
				}
				break;
		};
		
		selectedSkill = null;  
		EndPlayerTurn();
	}

	// Complete current player's turn and move to next
	public void EndPlayerTurn()
	{
		uiManager.HideAllMenus();
		turnIndex++;
		StartTurn();
	}

	// Execute AI actions for all living enemies
	private async void EnemyTurn()
	{
		activityIndicator?.Log("Enemy turn starting...");
		
		foreach (var enemy in enemyParty)
		{
			if (enemy.IsDead()) continue;
			
			await ToSignal(GetTree().CreateTimer(1.0), "timeout");
			enemy.PerformAI(playerParty); 
		}

		activityIndicator?.Log("Enemy turn complete, switching to player turn");
	
		await WaitForActivityIndicatorToFinish();

		isPlayerTurn = true;
		turnIndex = 0;
		StartTurn();
	}

	// Wait for all activity messages to finish displaying
	private async System.Threading.Tasks.Task WaitForActivityIndicatorToFinish()
	{
		while (activityIndicator.IsProcessing())
		{
			await ToSignal(GetTree().CreateTimer(0.1), "timeout");
		}
		
		await ToSignal(GetTree().CreateTimer(0.2), "timeout");
	}

	// Roll against character's speed to determine retreat success
	private bool TryRetreat(Character character)
	{
		int roll = GD.RandRange(0, 100);
		return roll < character.Speed;
	}

	private enum BattleResult
	{
		Victory,
		Defeat,
		Retreat
	}

	// End battle and transition to appropriate scene
	private void EndBattle(BattleResult result)
	{
		switch (result)
		{
			case BattleResult.Victory:
				activityIndicator?.Log("Victory!");
				GetTree().ChangeSceneToFile("res://Scenes/VictoryScreen.tscn");
				break;
				
			case BattleResult.Defeat:
				activityIndicator?.Log("Defeated!");
				GetTree().ChangeSceneToFile("res://Scenes/DefeatScreen.tscn");
				break;
				
			case BattleResult.Retreat:
				activityIndicator?.Log("Retreated successfully!");
				var gameManager = GameManager.Instance;
				if (gameManager != null && gameManager.HasBattleReturnPosition)
				{
					GetTree().ChangeSceneToFile("res://Scenes/Dungeon.tscn");
				}
				else
				{
					GetTree().ChangeSceneToFile("res://Scenes/Dungeon.tscn");
				}
				break;
		}
	}
}
