using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class VictoryScreen : Control
{
	[Export] private NodePath levelUpLabelPath;
	
	//Wildcard 1 Elements
	[Export] private NodePath wildCard1ButtonPath;
	[Export] private NodePath wildCard1NameLabelPath;
	[Export] private NodePath wildCard1DescriptionLabelPath;
	
	//Wildcard 2
	[Export] private NodePath wildCard2ButtonPath;
	[Export] private NodePath wildCard2NameLabelPath;
	[Export] private NodePath wildCard2DescriptionLabelPath;
	
	//Wildcard 3
	[Export] private NodePath wildCard3ButtonPath;
	[Export] private NodePath wildCard3NameLabelPath;
	[Export] private NodePath wildCard3DescriptionLabelPath;

	private Label levelUpLabel;
	
	private Button wildCard1Button;
	private Label wildCard1NameLabel;
	private Label wildCard1DescriptionLabel;
	
	private Button wildCard2Button;
	private Label wildCard2NameLabel;
	private Label wildCard2DescriptionLabel;
	
	private Button wildCard3Button;
	private Label wildCard3NameLabel;
	private Label wildCard3DescriptionLabel;

	private List<WildCardSystem.WildCard> offeredWildCards = new();

	public override void _Ready()
	{
		// Get UI references
		levelUpLabel = GetNode<Label>(levelUpLabelPath);
		
		wildCard1Button = GetNode<Button>(wildCard1ButtonPath);
		wildCard1NameLabel = GetNode<Label>(wildCard1NameLabelPath);
		wildCard1DescriptionLabel = GetNode<Label>(wildCard1DescriptionLabelPath);
		
		wildCard2Button = GetNode<Button>(wildCard2ButtonPath);
		wildCard2NameLabel = GetNode<Label>(wildCard2NameLabelPath);
		wildCard2DescriptionLabel = GetNode<Label>(wildCard2DescriptionLabelPath);
		
		wildCard3Button = GetNode<Button>(wildCard3ButtonPath);
		wildCard3NameLabel = GetNode<Label>(wildCard3NameLabelPath);
		wildCard3DescriptionLabel = GetNode<Label>(wildCard3DescriptionLabelPath);

		// Connect button signals
		wildCard1Button.Pressed += () => OnWildCardSelected(0);
		wildCard2Button.Pressed += () => OnWildCardSelected(1);
		wildCard3Button.Pressed += () => OnWildCardSelected(2);

		ProcessVictory();
	}

	private void ProcessVictory()
	{
		// Level up the party
		var gameManager = GameManager.Instance;
		if (gameManager?.SaveData != null)
		{
			gameManager.SaveData.PartyLevel++;
			levelUpLabel.Text = $"Level Up! Party is now Level {gameManager.SaveData.PartyLevel}!";
		}
		int goldReward = CalculateGoldReward();
		gameManager.SaveData.AddGold(goldReward);
		// Generate three random wild cards using the WildCardSystem
		GenerateWildCardOffers();
	}

	private void GenerateWildCardOffers()
	{
		// Use the WildCardSystem to generate random offers
		offeredWildCards = WildCardSystem.GenerateRandomOffers(3);

		//Display the wild cards
		if (offeredWildCards.Count >= 3)
		{
			PopulateWildCardUI(wildCard1NameLabel, wildCard1DescriptionLabel, wildCard1Button, offeredWildCards[0]);
			PopulateWildCardUI(wildCard2NameLabel, wildCard2DescriptionLabel, wildCard2Button, offeredWildCards[1]);
			PopulateWildCardUI(wildCard3NameLabel, wildCard3DescriptionLabel, wildCard3Button, offeredWildCards[2]);
		}
	}
	
	private int CalculateGoldReward()
	{
		var gameManager = GameManager.Instance;
		if (gameManager?.SaveData != null)
		{
			int floor = gameManager.SaveData.Floor;
			int baseReward = 20;
			int floorBonus = floor * 5; // More gold for higher floors
			int totalReward = baseReward + floorBonus;
		
		// Boss floors give extra gold
	   var floorType = DungeonManager.FloorHelper.GetFloorType(floor);
			if (floorType == DungeonManager.FloorType.Boss)
			{
				totalReward += 30; // Bonus for boss victories
			}
		
			return Math.Max(totalReward, 10); // Minimum 10 gold
		}
	
		return 20; // Default reward
	}

	private void PopulateWildCardUI(Label nameLabel, Label descriptionLabel, Button button, WildCardSystem.WildCard wildCard)
	{
		nameLabel.Text = wildCard.Name;
		descriptionLabel.Text = wildCard.Description;
		
		// Load and set the texture for the button
		LoadWildCardTexture(button, wildCard.Name);
	}

	private void LoadWildCardTexture(Button button, string wildCardName)
	{
		// Construct the path to the texture file
		string texturePath = $"res://Assets/WildCards/{wildCardName}.png";
		
		// Load the texture
		var texture = GD.Load<Texture2D>(texturePath);
			
		if (texture != null)
		{
			button.Icon = texture;
		}
		
	}
	//Adds the selected wild card to your save then returns you to the dungeon
	private void OnWildCardSelected(int index)
	{
		if (index >= 0 && index < offeredWildCards.Count)
		{
			var selectedWildCard = offeredWildCards[index];
			var gameManager = GameManager.Instance;
			if (gameManager?.SaveData != null)
			{
				if (!gameManager.SaveData.WildCards.ContainsKey(selectedWildCard.Name))
				{
					gameManager.SaveData.WildCards[selectedWildCard.Name] = 0;
				}
				gameManager.SaveData.WildCards[selectedWildCard.Name]++;
				gameManager.SaveGame();
			}

			
			GetTree().ChangeSceneToFile("res://Scenes/Dungeon.tscn");
		}
	}
}
