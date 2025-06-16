using Godot;
using System;

public partial class SettingsScreen : Control
{
	private HSlider musicVolumeSlider;
	private Label musicVolumeLabel;

	public override void _Ready()
	{
		//Grabs slider node
		musicVolumeSlider = GetNode<HSlider>("MusicVolumeSlider");
		
		//Grabs label node
		musicVolumeLabel = GetNodeOrNull<Label>("Volume");

		//Configure slider
		musicVolumeSlider.MinValue = 0.0;
		musicVolumeSlider.MaxValue = 1.0;
		musicVolumeSlider.Step = 0.01;

		//Connect slider signal
		musicVolumeSlider.ValueChanged += OnMusicVolumeChanged;
		
		//Load current volume value
		if (MusicVolumeManager.Instance != null)
		{
			musicVolumeSlider.Value = MusicVolumeManager.Instance.MusicVolume;
			UpdateVolumeLabel(MusicVolumeManager.Instance.MusicVolume);
		}
		var backButton = GetNode<Button>("BackButton");
		backButton.Pressed += () =>
		{
			GetTree().ChangeSceneToFile("res://Scenes/StartScreen.tscn");
		};
	}
	//Changes volume to the new value selected
	private void OnMusicVolumeChanged(double value)
	{
		float volume = (float)value;
		
		if (MusicVolumeManager.Instance != null)
		{
			MusicVolumeManager.Instance.SetMusicVolume(volume);
		}
		
		UpdateVolumeLabel(volume);
	}
	//Changes the displayed text to the new volume
	private void UpdateVolumeLabel(float volume)
	{
		if (musicVolumeLabel != null)
		{
			int percentage = Mathf.RoundToInt(volume * 100);
			musicVolumeLabel.Text = $"Music Volume: {percentage}%";
		}
	}
}
