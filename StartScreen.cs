using Godot;
using System;

public partial class StartScreen : Control
{
	//Basic Godot function that performs instructions upon loading the scene
	public override void _Ready()
	{
		//Grabs the button node and hooks it up to a method
		GetNode<Button>("CenterContainer/VBoxContainer/StartButton").Pressed += OnStartButtonPressed;
		GetNode<Button>("CenterContainer/VBoxContainer/SettingsButton").Pressed += OnSettingsButtonPressed;
		GetNode<Button>("CenterContainer/VBoxContainer/QuitButton").Pressed += OnQuitButtonPressed;
	}
	//Hooks up to the button presses
	private void OnStartButtonPressed()
	{
		GD.Print("start initialized");
		//Changes the scene to the specified file location
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
