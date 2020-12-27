﻿using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name => clip.name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1;
    [Range(.1f, 3)]
    public float pitch = 1;

    public bool loop = false;

    [HideInInspector]
    public AudioSource source;

}