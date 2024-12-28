using Godot;
using System;

public partial class SFXController : Node
{
	[Export] public int numChannels;
	public static SFXController sfx;
    public override void _Ready()
    {
		sfx = this;
		for(int i = 0; i < numChannels; i++) {
			AudioStreamPlayer player = new AudioStreamPlayer();
			AddChild(player);
		}
    }

	//Call this with a loaded sound to play it
    public static AudioStreamPlayer PlaySound(AudioStream sound) {
		foreach(var node in sfx.GetChildren()) {
			AudioStreamPlayer audio = node as AudioStreamPlayer;
			if(!audio.Playing) {
				audio.Stream = sound;
				audio.Play();
				return audio;
			}
		}
		AudioStreamPlayer oldestAudio = sfx.GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		float longestTimePlayed = -1;
		foreach(var node in sfx.GetChildren()) {
			AudioStreamPlayer audio = node as AudioStreamPlayer;
			float thisTime = audio.GetPlaybackPosition();
			if(thisTime > longestTimePlayed) {
				longestTimePlayed = thisTime;
				oldestAudio = audio;
			}
		}
		oldestAudio.Stop();
		oldestAudio.Stream = sound;
		oldestAudio.Play();
		return oldestAudio;
	}
}
