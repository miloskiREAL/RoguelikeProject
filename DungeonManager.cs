using Godot;
using System;

public partial class DungeonManager : Node2D
{
	public override void _Ready()
	{
		int floor = GameManager.Instance.SaveData.Floor;
		FloorType type = GetFloorType(floor);
		int block = GetBlock(floor);
		switch (type)
		{
			case FloorType.Start:
				GetTree().ChangeSceneToFile("res://Scenes/DungeonStart.tscn");
				break;
			case FloorType.Dungeon:
				GetTree().ChangeSceneToFile($"res://Scenes/Dungeon{block}.tscn");
		}
	}
	
}
