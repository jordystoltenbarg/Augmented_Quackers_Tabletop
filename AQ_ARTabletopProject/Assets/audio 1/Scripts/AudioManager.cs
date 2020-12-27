using UnityEditor.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake() {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
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
}
