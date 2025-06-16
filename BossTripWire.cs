using Godot;
using System;

public partial class BossTripWire : Area2D
{
	[Export] public int BossBlock = 1; // Which block's boss to fight (1-4)
	[Export] public float CooldownTime = 1.0f; // Time in seconds before trigger can activate again
	
	private bool hasTriggered = false;
	private Player player;
	private float cooldownTimer = 0.0f;
	private string triggerKey; // Unique key for this trigger something like this "floor_block"
	
	public override void _Ready()
	{
		// Connect area signals
		BodyEntered += OnPlayerEntered;
		BodyExited += OnPlayerExited;
		
		// Create unique key for this trigger based on current floor and boss block
		int currentFloor = GameManager.Instance.SaveData.Floor;
		triggerKey = $"{currentFloor}_{BossBlock}";
		
		// Check if this specific boss trigger has already been activated on this floor
		if (GameManager.Instance.SaveData.TriggeredBossWires.ContainsKey(triggerKey) && 
			GameManager.Instance.SaveData.TriggeredBossWires[triggerKey])
		{
			hasTriggered = true;
			GD.Print($"Boss trip wire already triggered on floor {currentFloor} for block {BossBlock} - staying disabled");
		}
		
		// Start with a cooldown if we just returned from battle
		if (GameManager.Instance.HasBattleReturnPosition)
		{
			cooldownTimer = CooldownTime;
			GD.Print("Boss trip wire on cooldown after battle return");
		}
	}
	
	public override void _Process(double delta)
	{
		if (cooldownTimer > 0)
		{
			cooldownTimer -= (float)delta;
		}
		
		// Check if we've moved to a different floor, if so, we can reset for this new floor
		int currentFloor = GameManager.Instance.SaveData.Floor;
		string currentTriggerKey = $"{currentFloor}_{BossBlock}";
		
		if (triggerKey != currentTriggerKey)
		{
			// We're on a different floor now, update our trigger key
			triggerKey = currentTriggerKey;
			
			// Check if this boss has been triggered on this new floor
			if (GameManager.Instance.SaveData.TriggeredBossWires.ContainsKey(triggerKey) && 
				GameManager.Instance.SaveData.TriggeredBossWires[triggerKey])
			{
				hasTriggered = true;
				GD.Print($"Boss trip wire already triggered on new floor {currentFloor} for block {BossBlock}");
			}
			else
			{
				hasTriggered = false;
				GD.Print($"Boss trip wire reset for new floor {currentFloor} block {BossBlock}");
			}
		}
	}
	
	private void OnPlayerEntered(Node body)
	{
		if (body is Player p && !hasTriggered && cooldownTimer <= 0)
		{
			player = p;
			TriggerBossEncounter();
		}
	}
	
	private void OnPlayerExited(Node body)
	{
		if (body is Player)
		{
			player = null;
			GD.Print("Player left boss trip wire area");
		}
	}
	
	private void TriggerBossEncounter()
	{
		if (hasTriggered) return;
		
		hasTriggered = true;
		
		// Mark this trigger as used in the save data
		GameManager.Instance.SaveData.TriggeredBossWires[triggerKey] = true;
		
		// Save the game to persist this information
		GameManager.Instance.SaveGame();
		
		// Save current position for battle return
		GameManager.Instance.QuickSaveForBattle(player.GlobalPosition);
		
		// Load boss battle scene
		string battleScene = $"res://Scenes/BossBattle{BossBlock}.tscn";
		GD.Print($"Boss trip wire triggered for {triggerKey}! Loading scene: {battleScene}");
		
		GetTree().ChangeSceneToFile(battleScene);
	}
	
	// Method to manually reset the trigger
	public void ResetTrigger()
	{
		hasTriggered = false;
		cooldownTimer = 0.0f;
		
		// Also clear from save data
		if (GameManager.Instance.SaveData.TriggeredBossWires.ContainsKey(triggerKey))
		{
			GameManager.Instance.SaveData.TriggeredBossWires[triggerKey] = false;
		}
		
		GD.Print($"Boss trip wire manually reset for {triggerKey}");
	}
	
	// Method to check if this trigger has already been activated
	public bool HasBeenTriggered()
	{
		return hasTriggered;
	}
}
