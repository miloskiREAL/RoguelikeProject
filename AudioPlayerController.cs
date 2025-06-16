using Godot;
using System;

public partial class AudioPlayerController : AudioStreamPlayer
{
	private float baseVolume = 0.0f; 

	public override void _Ready()
	{
		// Store the original volume
		baseVolume = VolumeDb;
		
		// Connect to volume manager when ready
		CallDeferred(nameof(ConnectToVolumeManager));
	}

	private void ConnectToVolumeManager()
	{
		if (MusicVolumeManager.Instance == null)
		{
			// Trys again if connection failed
			GetTree().CreateTimer(0.1f).Timeout += ConnectToVolumeManager;
			return;
		}

		// Connect to volume change signal
		MusicVolumeManager.Instance.MusicVolumeChanged += OnVolumeChanged;
		
		// Apply current volume immediately
		OnVolumeChanged(MusicVolumeManager.Instance.MusicVolume);
	}

	private void OnVolumeChanged(float sliderValue)
	{
		float minVolumeDb = -40.0f; // Inaudible, minimum volume
		float maxVolumeDb = 0.0f;   // Normal volume
		
		float volumeDb = baseVolume + Mathf.Lerp(minVolumeDb, maxVolumeDb, sliderValue);
		VolumeDb = volumeDb;
	}

	public override void _ExitTree()
	{
		// Disconnect to prevent memory leaks
		if (MusicVolumeManager.Instance != null)
		{
			MusicVolumeManager.Instance.MusicVolumeChanged -= OnVolumeChanged;
		}
	}
}
