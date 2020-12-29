using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixers")]
    [SerializeField] private AudioMixerGroup _sfxAudioMixer;
    [SerializeField] private AudioMixerGroup _musicAudioMixer;
    [SerializeField] private AudioMixerGroup _dialogueAudioMixer;
    [Header("Music")]
    [SerializeField] [Tooltip("If set to below 0, first music played will be random")] private int _firstMusicIndex = 0;
    private readonly List<Sound> _bgMusicList = new List<Sound>();
    private int _currentMusicIndex = 0;
    [Header("Sounds")]
    public Sound[] sounds;

    private void Awake()
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

            if (s.SoundType == SoundType.Music)
            {
                _bgMusicList.Add(s);
            }
        }
    }

    private void Start()
    {
        Invoke(nameof(playFirstSong), 1.0f);
    }

    private void playFirstSong()
    {
        if (_firstMusicIndex < 0)
            _firstMusicIndex = UnityEngine.Random.Range(0, _bgMusicList.Count);
        _currentMusicIndex = _firstMusicIndex;
        Play(_bgMusicList[_currentMusicIndex].clip);
        StartCoroutine(playNextMusic(_bgMusicList[_currentMusicIndex].clip.length));
    }

    private IEnumerator playNextMusic(float pCurrentMusicLength)
    {
        yield return new WaitForSeconds(pCurrentMusicLength);

        _currentMusicIndex++;
        if (_currentMusicIndex >= _bgMusicList.Count)
        {
            _currentMusicIndex = 0;
        }
        Play(_bgMusicList[_currentMusicIndex].clip);
        StartCoroutine(playNextMusic(_bgMusicList[_currentMusicIndex].clip.length));
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
