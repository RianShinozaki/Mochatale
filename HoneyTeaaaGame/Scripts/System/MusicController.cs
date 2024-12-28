using Godot;
using System;

public partial class MusicController : Node
{
	[Signal]
	public delegate void MusicStoppedEventHandler();
	public static MusicController music;
	public AudioStreamPlayer audioStreamPlayer;
	public bool inTransition;

	Tween tween;
	public override void _Ready()
	{
		music = this;
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
	}

	public static void StartMusic(AudioStream audio, float fadeTime = 0.5f, float desiredVolume = 0f) {
		music.StartMusicInst(audio, fadeTime, desiredVolume);
	}
	public async void StartMusicInst(AudioStream audio, float fadeTime, float desiredVolume) {
		if(music.audioStreamPlayer.Playing) {
			if(!inTransition)
				StopMusicInst(fadeTime);
			await ToSignal(this, SignalName.MusicStopped);
		}
		audioStreamPlayer.VolumeDb = -80f;
		audioStreamPlayer.Stream = audio;
		audioStreamPlayer.Play();
		tween = GetTree().CreateTween().BindNode(music).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(audioStreamPlayer, "volume_db", desiredVolume, fadeTime);
	}
	public static void StopMusic(float fadeTime = 4f) {
		music.StopMusicInst(fadeTime);
	}
	public async void StopMusicInst(float fadeTime) {
		inTransition = true;
		tween = GetTree().CreateTween().BindNode(music).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(audioStreamPlayer, "volume_db", -80f, fadeTime);
		await ToSignal(tween, "finished");
		audioStreamPlayer.Stop();
		inTransition = false;
		EmitSignal(SignalName.MusicStopped);
	}
}
