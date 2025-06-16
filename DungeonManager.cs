using Godot;
using System;
using System.Threading.Tasks;  

public partial class DungeonManager : Node2D
{
	public override async void _Ready()
	{
		int floor = GameManager.Instance.SaveData.Floor;
		
		// Check for floor changes and clear opened chests if floor changed
		GameManager.Instance.SaveData.CheckFloorChange();
		
		// Check if we've reached floor 11, if so, reset to floor 0 and clear boss tripwires
		if (floor == 11)
		{
			GD.Print("Floor 11 reached! Resetting to floor 0 and clearing all boss tripwires.");
			GameManager.Instance.SaveData.Floor = 0;
			GameManager.Instance.SaveData.TriggeredBossWires.Clear();
			// Also clear current floor chests since we're resetting
			GameManager.Instance.SaveData.OpenedChestsCurrentFloor.Clear();
			// Save the reset state
			GameManager.Instance.SaveGame(); 
			// Update local variable for the rest of this method
			floor = 0; 
		}
		
		FloorType type = FloorHelper.GetFloorType(floor);
		int block = FloorHelper.GetBlock(floor);
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		switch (type)
		{
			case FloorType.Start:
				GD.Print("switching to dungeon start");
				GetTree().ChangeSceneToFile("res://Scenes/DungeonStart.tscn");
				break;
			case FloorType.Dungeon:
				LoadDungeonLayout(block);
				break;
			case FloorType.Boss:
				GetTree().ChangeSceneToFile($"res://Scenes/Boss{block}Floor.tscn");
				break;
			case FloorType.Rest:
				GetTree().ChangeSceneToFile($"res://Scenes/Rest{block}Floor.tscn");
				break;
			case FloorType.FinalBoss:
				GetTree().ChangeSceneToFile($"res://Scenes/FinalBossFloor.tscn");
				break;
		}
	}
	
	private void LoadDungeonLayout(int block)
	{
		int layoutToUse;
		
		// If returning from battle, use the stored layout
		if (GameManager.Instance.HasBattleReturnPosition && GameManager.Instance.SaveData.CurrentDungeonLayout > 0)
		{
			layoutToUse = GameManager.Instance.SaveData.CurrentDungeonLayout;
			GD.Print($"Returning from battle, using stored layout: {layoutToUse}");
		}
		// If entering a new dungeon or no layout stored, generate a new random layout
		else
		{
			layoutToUse = GD.RandRange(1, 4);
			GameManager.Instance.SaveData.CurrentDungeonLayout = layoutToUse;
			GD.Print($"New dungeon floor, generated layout: {layoutToUse}");
		}
		
		GetTree().ChangeSceneToFile($"res://Scenes/Dungeon{block}FloorLayout{layoutToUse}.tscn");
	}
	//All of the possible floortypes
	public enum FloorType { Start, Dungeon, Boss, Rest, FinalBoss }
	
	public static class FloorHelper
	{
		//Finds the current floortype based on floor number
		public static FloorType GetFloorType(int floor)
		{
			if (floor == 0)
			{
				return FloorType.Start;
			}
			else if (floor == 10)
			{
				return FloorType.FinalBoss;
			}
			
			
			int posCycle = (floor - 1) % 3;
			
			if (posCycle == 0)
			{
				return FloorType.Dungeon;
			}
			
			if (posCycle == 1)
			{
				return FloorType.Boss;
			}
			
			if (posCycle == 2)
			{
				return FloorType.Rest;
			}
			return FloorType.Start;
		}
	 	//Returns a number from 0-4
		public static int GetBlock(int floor)
		{
			if (floor == 0 || floor == 11){
				return 0;
			}

			int block = ((floor - 1) / 3) + 1;
			return block;
		}
	}
	
}
