using Godot;
using System;

public partial class StartScreen : Control
{
	public override async void _Ready()
	{
		await ToSignal(GetTree(), "process_frame");
		MusicManager.Instance.StopMusic();
		var startMusic = GD.Load<AudioStream>("res://audio/start/startscreen.wav");
		MusicManager.Instance.PlayMusic(startMusic);
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
