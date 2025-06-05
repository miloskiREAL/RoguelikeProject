using Godot;
using System;
using System.Collections.Generic; 

public partial class BattleManager : Control
{
	private List<Character> playerParty;
	private List<Character> enemyParty;
	private List<Character> turnOrder = new();
	private int currentTurnIndex = 0;
	public override void _Ready()
	{
		LoadPlayerParty();
		LoadEnemyParty();
		StartBattle();
	}

	private void LoadPlayerParty()
	{
		playerParty = new List<Character>
		{
		GetNode<Character>("PlayerParty/Knight"),
		GetNode<Character>("PlayerParty/Wizard"),
		GetNode<Character>("PlayerParty/Monk"),
		GetNode<Character>("PlayerParty/Ranger")
		};

		foreach (var player in playerParty)
		{
			player.Init(player.CharacterClass);
		}
	}

	private void LoadEnemyParty()
	{
		
		int block = GameManager.Instance.SaveData.Floor; 
		var enemies = EnemyPool.GetRandomEnemiesForBlock(block, 1, 4);
		enemyParty = new List<Character>();

		for (int i = 0; i < enemyScenes.Count; i++)
		{
			var enemyScene = enemyScenes[i];
			Character enemy = enemyScene.Instantiate<Character>();
			AddChild(enemy); 
			enemyParty.Add(enemy);
			enemy.Init(enemy.CharacterClass); 
		}
	}

	private void StartBattle()
	{
		turnOrder.Clear();
		turnOrder.AddRange(playerParty); 
		turnOrder.AddRange(enemyParty);
	}

	private void NextTurn()
	{
		
	}

	private void EndBattle(bool playerWon)
	{
		
	}
}
