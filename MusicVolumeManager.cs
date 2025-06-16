using Godot;
using System;

public partial class MusicVolumeManager : Node
{
	public static MusicVolumeManager Instance { get; private set; }
	
	public float MusicVolume { get; private set; } = 1.0f;
	
	// Signal for volume changes
	[Signal]
	public delegate void MusicVolumeChangedEventHandler(float newVolume);

	public override void _Ready()
	{
		// Singleton pattern
		if (Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}

		Instance = this;
		LoadVolumeSettings();
	}

	public void SetMusicVolume(float volume)
	{
		MusicVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
		EmitSignal(SignalName.MusicVolumeChanged, MusicVolume);
		SaveVolumeSettings();
	}

	private void SaveVolumeSettings()
	{
		var config = new ConfigFile();
		config.SetValue("audio", "music_volume", MusicVolume);
		config.Save("user://audio_settings.cfg");
	}

	private void LoadVolumeSettings()
	{
		var config = new ConfigFile();
		if (config.Load("user://audio_settings.cfg") == Error.Ok)
		{
			MusicVolume = (float)config.GetValue("audio", "music_volume", 1.0f);
		}
	}
}
