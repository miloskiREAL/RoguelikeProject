using Godot;
using System;
using System.Collections.Generic;

public partial class MusicManager : Node
{
	// Singleton instance that can be accessed from anywhere
	public static MusicManager Instance { get; private set; }
	
	// Main audio player for music
	private AudioStreamPlayer _musicPlayer;
	
	// Dictionary to store preloaded music
	private Dictionary<string, AudioStream> _musicTracks = new Dictionary<string, AudioStream>();
	
	// Current state tracking
	private string _currentTrackName;
	
	// Configuration options
	[Export] public float DefaultVolume { get; set; } = -20.0f; // in dB
	[Export] public bool PlayMusicOnStart { get; set; } = true;
	[Export] public string DefaultMusicPath { get; set; } = "res://audio/start/startscreen.wav";
	
	// Constructor - set instance immediately
	public MusicManager()
	{
		// Set the singleton instance right away
		Instance = this;
	}
	
	public override void _Ready()
	{
		GD.Print("MusicManager initializing...");
		
		// Handle singleton pattern
		if (Instance != this)
		{
			GD.PrintErr("Multiple MusicManager instances detected!");
			QueueFree();
			return;
		}
		
		// Get the AudioStreamPlayer
		_musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
		
		if (_musicPlayer == null)
		{
			GD.PrintErr("MusicPlayer node not found! Add an AudioStreamPlayer named 'MusicPlayer' as a child.");
			return;
		}
		
		// Set initial volume
		_musicPlayer.VolumeDb = DefaultVolume;
		
		// Preload default music
		PreloadMusic();
		
		// Play default music if enabled
		if (PlayMusicOnStart && !string.IsNullOrEmpty(DefaultMusicPath))
		{
			PlayMusicByPath(DefaultMusicPath);
		}
		
		GD.Print("MusicManager initialized successfully.");
	}  // <-- This closing brace was missing
	
	// Preload music method that was referenced but missing in original code
	private void PreloadMusic()
	{
		// You can add code here to preload your commonly used music tracks
		if (!string.IsNullOrEmpty(DefaultMusicPath))
		{
			// At minimum, preload the default track
			var defaultMusic = GD.Load<AudioStream>(DefaultMusicPath);
			if (defaultMusic != null)
			{
				_musicTracks[DefaultMusicPath] = defaultMusic;
			}
		}
	}

	// Play music by file path
	public void PlayMusicByPath(string path, bool loop = true)
	{
		if (_musicPlayer == null)
		{
			GD.PrintErr("MusicPlayer not initialized!");
			return;
		}
		
		AudioStream music;
		
		// Check if we've already loaded this track
		if (_musicTracks.ContainsKey(path))
		{
			music = _musicTracks[path];
		}
		else
		{
			// Load the music file
			music = GD.Load<AudioStream>(path);
			
			if (music == null)
			{
				GD.PrintErr($"Failed to load music from path: {path}");
				return;
			}
			
			// Store for future use
			_musicTracks[path] = music;
		}
		
		// Play the music
		PlayMusic(music, loop);
		_currentTrackName = path;
	}
	
	// Play music by reference
	public void PlayMusic(AudioStream music, bool loop = true)
	{
		if (_musicPlayer == null)
		{
			GD.PrintErr("MusicPlayer not initialized!");
			return;
		}
		
		// Don't restart if it's already playing
		if (_musicPlayer.Stream == music && _musicPlayer.Playing)
		{
			return;
		}
		
		// Configure looping based on audio type
		SetLooping(music, loop);
		
		// Play the music
		_musicPlayer.Stream = music;
		_musicPlayer.Play();
		
		GD.Print("Now playing music");
	}
	
	// Helper to set looping based on audio type
	private void SetLooping(AudioStream music, bool loop)
	{
		if (!loop) return; // No need to configure if not looping
		
		if (music is AudioStreamWav wav)
		{
			wav.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
		}
	}
	
	// Stop currently playing music
	public void StopMusic()
	{
		if (_musicPlayer == null) return;
		
		_musicPlayer.Stop();
		_currentTrackName = null;
	}
	
	// Pause currently playing music
	public void PauseMusic()
	{
		if (_musicPlayer == null) return;
		
		_musicPlayer.StreamPaused = true;
	}
	
	// Resume paused music
	public void ResumeMusic()
	{
		if (_musicPlayer == null) return;
		
		_musicPlayer.StreamPaused = false;
	}
	
	// Fade out the current music
	public void FadeOutMusic(float duration = 1.0f)
	{
		if (_musicPlayer == null || !_musicPlayer.Playing) return;
		
		Tween tween = CreateTween();
		tween.TweenProperty(_musicPlayer, "volume_db", -80.0, duration);
		tween.TweenCallback(Callable.From(() => StopMusic()));
	}
	
	// Set music volume
	public void SetVolume(float volumeDb)
	{
		if (_musicPlayer == null) return;
		
		_musicPlayer.VolumeDb = volumeDb;
	}
	
	// Get the currently playing track name
	public string GetCurrentTrack()
	{
		return _currentTrackName;
	}
}
