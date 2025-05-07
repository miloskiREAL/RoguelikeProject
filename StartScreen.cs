using Godot;
using System;

public partial class StartScreen : Control
{
	public override void _Ready()
	{
		GetNode<Button>("CenterContainer/VBoxContainer/StartButton").Pressed += OnStartButtonPressed;
		GetNode<Button>("CenterContainer/VBoxContainer/SettingsButton").Pressed += OnSettingsButtonPressed;
		GetNode<Button>("CenterContainer/VBoxContainer/QuitButton").Pressed += OnQuitButtonPressed;
	}
	
	private void OnStartButtonPressed()
	{
		GD.Print("start initialized");
		GetTree().ChangeSceneToFile("res://Scenes/SaveLoadScreen.tscn");
	}
	
	private void OnSettingsButtonPressed()
	{
		GD.Print("settings screen");
		GetTree().ChangeSceneToFile("res://Scenes/SettingsScreen.tscn");
	}
	
	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
