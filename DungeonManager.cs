using Godot;
using System;

public partial class DungeonManager : Node2D
{
	public override void _Ready()
	{
		GetTree().ChangeSceneToFile("res://Scenes/DungeonStart.tscn");
		int floor = GameManager.Instance.SaveData.Floor;
		FloorType type = FloorHelper.GetFloorType(floor);
		int block = FloorHelper.GetBlock(floor);
		switch (type)
		{
			case FloorType.Start:
				GetTree().ChangeSceneToFile("res://Scenes/DungeonStart.tscn");
				GD.Print("switching to dungeon start");
				break;
			case FloorType.Dungeon:
				GetTree().ChangeSceneToFile($"res://Scenes/Dungeon{block}Floor.tscn");
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
	
	public enum FloorType { Start, Dungeon, Boss, Rest, FinalBoss }
	
	public static class FloorHelper
	{
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
			
			//fallthrough incase anything bad happens
			return FloorType.Start;
		}
	
		public static int GetBlock(int floor)
		{
			if (floor == 0 || floor == 10){
				return 0;
			}

			int block = ((floor - 1) / 3) + 1;
			return block;
		}
	}
	
}
