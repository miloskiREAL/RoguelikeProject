using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Shop : Control
{
	[Export] private NodePath shopTitleLabelPath;
	[Export] private NodePath goldLabelPath;
	[Export] private NodePath backButtonPath;
	
	// Item 1
	[Export] private NodePath item1ButtonPath;
	[Export] private NodePath item1NameLabelPath;
	[Export] private NodePath item1DescriptionLabelPath;
	[Export] private NodePath item1CostLabelPath;
	
	// Item 2
	[Export] private NodePath item2ButtonPath;
	[Export] private NodePath item2NameLabelPath;
	[Export] private NodePath item2DescriptionLabelPath;
	[Export] private NodePath item2CostLabelPath;
	
	// Item 3
	[Export] private NodePath item3ButtonPath;
	[Export] private NodePath item3NameLabelPath;
	[Export] private NodePath item3DescriptionLabelPath;
	[Export] private NodePath item3CostLabelPath;

	private Label shopTitleLabel;
	private Label goldLabel;
	private Button backButton;
	
	private Button item1Button;
	private Label item1NameLabel;
	private Label item1DescriptionLabel;
	private Label item1CostLabel;
	
	private Button item2Button;
	private Label item2NameLabel;
	private Label item2DescriptionLabel;
	private Label item2CostLabel;
	
	private Button item3Button;
	private Label item3NameLabel;
	private Label item3DescriptionLabel;
	private Label item3CostLabel;

	private List<ShopItem> currentShopItems = new();
	private int currentShopTier = 1;

	public override void _Ready()
	{
		// Get UI references
		shopTitleLabel = GetNode<Label>(shopTitleLabelPath);
		goldLabel = GetNode<Label>(goldLabelPath);
		backButton = GetNode<Button>(backButtonPath);
		
		item1Button = GetNode<Button>(item1ButtonPath);
		item1NameLabel = GetNode<Label>(item1NameLabelPath);
		item1DescriptionLabel = GetNode<Label>(item1DescriptionLabelPath);
		item1CostLabel = GetNode<Label>(item1CostLabelPath);
		
		item2Button = GetNode<Button>(item2ButtonPath);
		item2NameLabel = GetNode<Label>(item2NameLabelPath);
		item2DescriptionLabel = GetNode<Label>(item2DescriptionLabelPath);
		item2CostLabel = GetNode<Label>(item2CostLabelPath);
		
		item3Button = GetNode<Button>(item3ButtonPath);
		item3NameLabel = GetNode<Label>(item3NameLabelPath);
		item3DescriptionLabel = GetNode<Label>(item3DescriptionLabelPath);
		item3CostLabel = GetNode<Label>(item3CostLabelPath);

		// Connect button signals
		item1Button.Pressed += () => OnItemPurchased(0);
		item2Button.Pressed += () => OnItemPurchased(1);
		item3Button.Pressed += () => OnItemPurchased(2);
		backButton.Pressed += OnBackPressed;

		InitializeShop();
	}

	private void InitializeShop()
	{
		// Determine shop tier based on current floor/block
		var gameManager = GameManager.Instance;
		if (gameManager?.SaveData != null)
		{
			int floor = gameManager.SaveData.Floor;
			int block = DungeonManager.FloorHelper.GetBlock(floor);
			currentShopTier = block + 1; // Defines the current shop tier for the block e.g 0 --> 1
		}

		// Load shop items for this tier
		LoadShopItems();
		UpdateUI();
	}

	private void LoadShopItems()
	{
		currentShopItems.Clear();
		
		// Get all items as a list to work with indices
		var allItems = Item.All.Values.ToList();
		
		// Define shop items based on tier
		switch (currentShopTier)
		{
			case 1: // First shop: Potion, Ether, Mega Potion
				if (allItems.Count > 0) currentShopItems.Add(new ShopItem(allItems[0], 25));  // Potion
				if (allItems.Count > 1) currentShopItems.Add(new ShopItem(allItems[1], 40));  // Ether
				if (allItems.Count > 2) currentShopItems.Add(new ShopItem(allItems[2], 60));  // Mega Potion
				break;
				
			case 2: // Second shop: Mega Ether, Full Heal, Full Ether
				if (allItems.Count > 3) currentShopItems.Add(new ShopItem(allItems[3], 75));  // Mega Ether
				if (allItems.Count > 4) currentShopItems.Add(new ShopItem(allItems[4], 100)); // Full Heal
				if (allItems.Count > 5) currentShopItems.Add(new ShopItem(allItems[5], 120)); // Full Ether
				break;
			// Fallthrough due to 2 cases having same value
			case 3: // Third shop: Team Heal, Team Ether, Elixir
			case 4: // Fourth shop: Same as third (last three items)
			default:
				if (allItems.Count > 6) currentShopItems.Add(new ShopItem(allItems[6], 200)); // Team Heal
				if (allItems.Count > 7) currentShopItems.Add(new ShopItem(allItems[7], 250)); // Team Ether
				if (allItems.Count > 8) currentShopItems.Add(new ShopItem(allItems[8], 300)); // Elixir
				break;
		}
	}

	private void UpdateUI()
	{
		var gameManager = GameManager.Instance;
		if (gameManager?.SaveData != null)
		{
			shopTitleLabel.Text = $"Shop - Tier {currentShopTier}";
			goldLabel.Text = $"Gold: {gameManager.SaveData.Gold}";

			// Update item displays
			UpdateItemUI(item1Button, item1NameLabel, item1DescriptionLabel, item1CostLabel, 0);
			UpdateItemUI(item2Button, item2NameLabel, item2DescriptionLabel, item2CostLabel, 1);
			UpdateItemUI(item3Button, item3NameLabel, item3DescriptionLabel, item3CostLabel, 2);
		}
	}

	private void UpdateItemUI(Button button, Label nameLabel, Label descriptionLabel, Label costLabel, int index)
	{
		if (index < currentShopItems.Count)
		{
			var shopItem = currentShopItems[index];
			var item = shopItem.Item;
			
			nameLabel.Text = item.Name;
			descriptionLabel.Text = item.Description;
			costLabel.Text = $"{shopItem.Cost} Gold";
			
			// Check if player can afford the item
			var gameManager = GameManager.Instance;
			bool canAfford = gameManager?.SaveData != null && gameManager.SaveData.Gold >= shopItem.Cost;
			
			button.Disabled = !canAfford;
			button.Modulate = canAfford ? Colors.White : Colors.Gray;
			
			// Load item texture if available
			LoadItemTexture(button, item.Key);
		}
	}

	private void LoadItemTexture(Button button, string itemKey)
	{
		// Construct the path to the texture file
		string texturePath = $"res://Assets/Items/{itemKey}.png";
		// Load the texture
		var texture = GD.Load<Texture2D>(texturePath);
			
		if (texture != null)
		{
			button.Icon = texture;
		}

	}

	private void OnItemPurchased(int index)
	{
		if (index >= currentShopItems.Count) return;
		
		var shopItem = currentShopItems[index];
		var gameManager = GameManager.Instance;
		
		if (gameManager?.SaveData != null)
		{
			// Check if player has enough gold
			if (gameManager.SaveData.Gold >= shopItem.Cost)
			{
				// Deduct gold
				gameManager.SaveData.Gold -= shopItem.Cost;
				
				// Add item to inventory
				if (!gameManager.SaveData.Inventory.ContainsKey(shopItem.Item.Key))
				{
					gameManager.SaveData.Inventory[shopItem.Item.Key] = 0;
				}
				gameManager.SaveData.Inventory[shopItem.Item.Key]++;
				
				// Save the game
				gameManager.SaveGame();
				
				// Update UI to reflect changes
				UpdateUI();
				
				GD.Print($"Purchased {shopItem.Item.Name} for {shopItem.Cost} gold!");
			}
			else
			{
				GD.Print("Not enough gold!");
			}
		}
	}

	private void OnBackPressed()
	{
		// Return to dungeon manager to properly route back to current floor
		GetTree().ChangeSceneToFile("res://Scenes/Dungeon.tscn");
	}

	// Helper class to represent items in the shop with their costs
	public class ShopItem
	{
		public Item Item { get; set; }
		public int Cost { get; set; }

		public ShopItem(Item item, int cost)
		{
			Item = item;
			Cost = cost;
		}
	}
}
