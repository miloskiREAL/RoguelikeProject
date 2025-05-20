using Godot;
using System;

public partial class MusicManager : Node
{
	public static MusicManager Instance;

	private AudioStreamPlayer musicPlayer;

	public override void _Ready()
	{
		if (Instance != null)
		{
			QueueFree();
			return;
		}

		Instance = this;
		musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");

		
	}

	public void PlayMusic(AudioStream music, bool loop = true)
	{
		if (musicPlayer.Stream == music && musicPlayer.Playing)
		{
			return;
		}
		
		if (music is AudioStreamWav wav)
		{
			wav.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
		}
		musicPlayer.Stream = music;
		musicPlayer.Play();
	}

	public void StopMusic()
	{
		musicPlayer.Stop();
	}
}
