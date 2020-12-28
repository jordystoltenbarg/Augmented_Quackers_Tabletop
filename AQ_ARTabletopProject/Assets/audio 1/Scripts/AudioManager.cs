using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _sfxAudioMixer;
    [SerializeField] private AudioMixerGroup _musicAudioMixer;
    [SerializeField] private AudioMixerGroup _dialogueAudioMixer;
    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            switch (s.SoundType)
            {
                case SoundType.SFX:
                    s.source.outputAudioMixerGroup = _sfxAudioMixer;
                    break;
                case SoundType.Music:
                    s.source.outputAudioMixerGroup = _musicAudioMixer;
                    break;
                case SoundType.Dialogue:
                    s.source.outputAudioMixerGroup = _dialogueAudioMixer;
                    break;
            }
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(AudioClip clip)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == clip.name);
        if (s == null)
            return;
        s.source.Play();
    }

    public void Play(string pClipName)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == pClipName);
        if (s == null)
            return;
        s.source.Play();
    }
}
