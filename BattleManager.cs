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

	private void StartBattle()
	{
		turnIndex = 0;
		isPlayerTurn = true;
		activityIndicator.AddMessage("Battle begins!");
		StartTurn();
	}

	private void StartTurn()
	{
		if (isPlayerTurn)
		{			
			if (turnIndex >= playerParty.Count)
			{
				isPlayerTurn = false;
				turnIndex = 0;
				StartTurn();
				return;
			}

			var currentChar = playerParty[turnIndex];
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
					EndBattle(false);
				}
				else
				{
					activityIndicator.AddMessage($"{currentChar.CharacterName} couldn't escape!");
					EndPlayerTurn();
				}
				break;
		}
	}

	public void OnSkillSelected(Character.Skill skill)
	{
		selectedSkill = skill;
		uiManager.ShowTargetMenu(skill);
	}

	public void OnItemSelected(Item item, Character target)
	{
		if (item.IsTeamWide)
		{
			var aliveAllies = playerParty.Where(p => !p.IsDead()).ToList();
			foreach (var ally in aliveAllies)
			{
				item.UseOn(ally);
			}
			activityIndicator?.Log($"Used {item.Name} on all party members");
		}
		else
		{
			if (target != null)
			{
				item.UseOn(target);
				activityIndicator?.Log($"Used {item.Name} on {target.CharacterName}");
			}
			else
			{
				activityIndicator?.Log("No target specified for single-target item!");
			}
		}
	}

	public void OnTargetSelected(Character target, Character.Skill skill)
	{
		var user = playerParty[turnIndex];
		
		if (selectedSkill == null)
		{
			activityIndicator?.Log("No skill selected to activate!");
			return;
		}

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

	public void EndPlayerTurn()
	{
		uiManager.HideAllMenus();
		turnIndex++;
		StartTurn();
	}

	private async void EnemyTurn()
	{
		activityIndicator?.Log("Enemy turn starting...");
		
		foreach (var enemy in enemyParty)
		{
			if (enemy.IsDead()) continue;
			
			await ToSignal(GetTree().CreateTimer(1.0), "timeout");
			enemy.PerformAI(playerParty); 
		}

		isPlayerTurn = true;
		turnIndex = 0;
		activityIndicator?.Log("Enemy turn complete, switching to player turn");
		StartTurn();
	}

	private bool TryRetreat(Character character)
	{
		int roll = GD.RandRange(0, 100);
		return roll < character.Speed;
	}

	private void EndBattle(bool playerWon)
	{
		if (playerWon)
		{
			activityIndicator?.Log("Victory!");
		}
		else
		{
			activityIndicator?.Log("Retreated or Defeated");
		}

		GetTree().ChangeSceneToFile("res://Scenes/DungeonManager.tscn");
	}
}
