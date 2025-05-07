using Godot;
using System;

public partial class StartScreen : Control
{
	public override void _Ready()
	{
		GetNode<Button>("StartButton").Pressed += OnStartButtonPressed;
		GetNode<Button>("SettingsButton").Pressed += OnSettingsButtonPressed;
		GetNode<Button>("QuitButton").Pressed += OnQuitButtonPressed;
	}
	
	private void OnStartButtonPressed()
	{
		GD.Print("start initialized");
		GetTree.ChangeSceneToFile(res://);
	}
	
	private void OnSettingsButtonPressed()
	{
		GD.Print("settings screen")
		GetTree.ChangeSceneToFile();
	}
	
	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
