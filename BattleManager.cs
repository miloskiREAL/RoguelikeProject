using Godot;
using System;
using System.Collections.Generic;

public partial class BattleManager : Control
{
	private List<PlayerCharacter> playerParty = new();
	private List<Enemy> enemyParty = new(); 
	private int turnIndex = 0;
	private bool isPlayerTurn = true;

	[Export] private NodePath playerPartyContainerPath;
	[Export] private NodePath enemyPartyContainerPath;
	[Export] private NodePath uiManagerPath;

	private Control playerPartyContainer;
	private Control enemyPartyContainer;
	private UIManager uiManager;

	public override void _Ready()
	{
		playerPartyContainer = GetNode<Control>(playerPartyContainerPath);
		enemyPartyContainer = GetNode<Control>(enemyPartyContainerPath);
		uiManager = GetNode<UIManager>(uiManagerPath);

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
			character.Init(character.Class);
		}
	}

	private void LoadEnemyParty()
	{
		foreach (Enemy enemy in enemyPartyContainer.GetChildren())
		{
			enemyParty.Add(enemy); 
			enemy.Init(enemy.Class); 
		}
	}

	private void StartBattle()
	{
		turnIndex = 0;
		isPlayerTurn = true;
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
					EndBattle(false);
				else
					EndPlayerTurn();
				break;
		}
	}

	public void OnSkillSelected(Character.Skill skill)
	{
		uiManager.ShowTargetMenu(skill);
	}

	public void OnTargetSelected(Character target, Character.Skill skill)
	{
		var user = playerParty[turnIndex];
		skill.Activate(user, target);
		EndPlayerTurn();
	}

	private void EndPlayerTurn()
	{
		uiManager.HideAllMenus();
		turnIndex++;
		StartTurn();
	}

	private async void EnemyTurn()
	{
		foreach (var enemy in enemyParty)
		{
			if (enemy.IsDead()) continue;
			await ToSignal(GetTree().CreateTimer(1.0), "timeout");
			enemy.PerformAI(playerParty); 
		}

		isPlayerTurn = true;
		turnIndex = 0;
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
			GD.Print("Victory!");
			// Handle victory logic
		}
		else
		{
			GD.Print("Retreated or Defeated");
			// Handle loss or retreat
		}

		GetTree().ChangeSceneToFile("res://Scenes/DungeonManager.tscn");
	}
}
