using Godot;
using System;
using System.Collections.Generic;

public partial class BattleManager : Control
{
	private List<Character> playerParty = new();
	private List<Character> enemyParty = new();
	private int turnIndex = 0;
	private bool isPlayerTurn = true;

	[Export] private NodePath playerPartyContainerPath;
	[Export] private NodePath enemyPartyContainerPath;

	private VBoxContainer playerPartyContainer;
	private VBoxContainer enemyPartyContainer;

	public override void _Ready()
	{
		playerPartyContainer = GetNode<VBoxContainer>(playerPartyContainerPath);
		enemyPartyContainer = GetNode<VBoxContainer>(enemyPartyContainerPath);

		LoadPlayerParty();
		LoadEnemyParty();
		StartBattle();
	}

	private void LoadPlayerParty()
	{
		foreach (Character character in playerPartyContainer.GetChildren())
		{
			playerParty.Add(character);
			character.Init(character.Class);
		}
	}

	private void LoadEnemyParty()
	{
		int block = GameManager.Instance.SaveData.Floor;
		var enemies = EnemyPool.GetRandomEnemiesForBlock(block, 1, 4);

		foreach (var enemyScene in enemies)
		{
			var enemyInstance = (Character)enemyScene.Instantiate();
			enemyParty.Add(enemyInstance);
			enemyPartyContainer.AddChild(enemyInstance);
			enemyInstance.Init(enemyInstance.Class); // Initialize enemy stats
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

			UIManager.Instance.ShowActionButtons(currentChar);
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
				UIManager.Instance.ShowSkillMenu(currentChar);
				break;
			case "Item":
				UIManager.Instance.ShowItemMenu(currentChar);
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
		UIManager.Instance.ShowTargetMenu(skill);
	}

	public void OnTargetSelected(Character target, Character.Skill skill)
	{
		var user = playerParty[turnIndex];
		skill.Activate(user, target);
		EndPlayerTurn();
	}

	private void EndPlayerTurn()
	{
		UIManager.Instance.HideAllMenus();
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
