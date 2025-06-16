using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ChestInt : Node2D
{
	private bool inRange = false;
	private bool isOpened = false;
	//Unique identifier for this chest
	private string chestId; 
	
	//Visual components of the chest
	private AnimatedSprite2D exclamationSprite;
	private AnimatedSprite2D chestSprite;
	//The physical interact area for the chest
	private Area2D interactArea;
	//Defines the Block and items you can get there
	private static readonly Dictionary<int, List<string>> BlockItemPools = new()
	{
		{ 1, new List<string> { "Potion", "Ether", "Mega Potion" } }, 
		{ 2, new List<string> { "Mega Ether", "Full Heal", "Full Ether" } }, 
		{ 3, new List<string> { "Team Heal", "Team Ether", "Elixer" } }    
	};

	
	public override void _Ready()
	{
		//Generate unique chest ID based on scene and position (without floor number)
		chestId = GenerateChestId();
		
		// Get child nodes
		interactArea = GetNode<Area2D>("InteractArea");
		exclamationSprite = GetNode<AnimatedSprite2D>("InteractArea/Exclamation");
		chestSprite = GetNode<AnimatedSprite2D>("ChestSprite");
		
		//Connect area signals
		interactArea.BodyEntered += OnPlayerEnter;
		interactArea.BodyExited += OnPlayerExit;
		
		//Check if this chest has already been opened on the current floor
		isOpened = IsChestOpenedThisFloor();
		
		//Set initial animation state, if opened plays the opened if not plays the closed
		if (isOpened && chestSprite != null)
		{
			chestSprite.Play("opened");
		}
		else if (chestSprite != null)
		{
			chestSprite.Play("closed");
		}
		
		//Hide exclamation initially
		if (exclamationSprite != null)
		{
			exclamationSprite.Visible = false;
		}
	}
	
	private string GenerateChestId()
	{
		//Create unique ID based on current scene and chest position, looks like this res://Scenes/Dungeon2FloorLayout3.tscn_1248_360 ---> unique chest ID states position, and scene it is in

		string sceneName = GetTree().CurrentScene.SceneFilePath;
		Vector2 pos = GlobalPosition;
		return $"{sceneName}_{pos.X}_{pos.Y}";
	}
	
	private bool IsChestOpenedThisFloor()
	{
		//Check if chest is opened for the current floor, basically just searches for the unique identifier made once opened
		int currentFloor = GameManager.Instance.SaveData.Floor;
		string floorChestKey = $"Floor_{currentFloor}_{chestId}";
		return GameManager.Instance.SaveData.OpenedChestsCurrentFloor.Contains(floorChestKey);
	}
	
	private void OnPlayerEnter(Node body)
	{
		if (body is Player && !isOpened)
		{
			inRange = true;
			exclamationSprite.Visible = true;
		}
	}
	
	private void OnPlayerExit(Node body)
	{
		if (body is Player)
		{
			inRange = false;
			exclamationSprite.Visible = false;
		}
	}
	
	public override void _Process(double delta)
	{
		if (inRange && !isOpened && Input.IsActionJustPressed("interact"))
		{
			OpenChest();
		}
	}
	
	private void OpenChest()
	{
		//Mark this unique chest as opened 
		isOpened = true;
		int currentFloor = GameManager.Instance.SaveData.Floor;
		string floorChestKey = $"Floor_{currentFloor}_{chestId}";
		GameManager.Instance.SaveData.OpenedChestsCurrentFloor.Add(floorChestKey);
		
		//Update animations
		chestSprite.Play("opened");
		exclamationSprite.Visible = false;
		
		//Determine current block and get appropriate item
		int block = DungeonManager.FloorHelper.GetBlock(currentFloor);
		
		//Get random item from appropriate block pool
		string itemKey = GetRandomItemForBlock(block);
		
		if (itemKey != null && Item.All.ContainsKey(itemKey))
		{
			//Add item to inventory
			GameManager.Instance.SaveData.AddItem(itemKey, 1);
			
			//Get item info to be printed
			Item item = Item.All[itemKey];
			GD.Print($"Chest opened! Found: {item.Name} (Block {block}, Floor {currentFloor})");
			//Save the game to save the added item and opened id to memory
			GameManager.Instance.SaveGame();
		}
	}
	
	private string GetRandomItemForBlock(int block)
	{
		//Clamp block to available pools, essentially just keeps the value between (1, 3)
		block = Mathf.Clamp(block, 1, 3);
		
		if (BlockItemPools.ContainsKey(block) && BlockItemPools[block].Count > 0)
		{
			//Generates a random number to use as an index and returns whatever value is held at that index for the current block
			var itemPool = BlockItemPools[block];
			int randomIndex = GD.RandRange(0, itemPool.Count - 1);
			return itemPool[randomIndex];
		}
		
		return null;
	}
}
