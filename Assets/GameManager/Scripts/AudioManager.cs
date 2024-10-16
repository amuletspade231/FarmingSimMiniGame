using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 0.5f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop;
        public bool playOnAwake;  // New field for Play on Awake
        [HideInInspector] public AudioSource source;
    }

    public List<Sound> sounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;

            // Play the sound if playOnAwake is true
            if (sound.playOnAwake)
            {
                sound.source.Play();
            }
        }
    }

    public void Play(string soundName)
    {
        Sound sound = sounds.Find(s => s.name == soundName);
        if (sound != null)
        {
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning($"Sound {soundName} not found!");
        }
    }

    public void Stop(string soundName)
    {
        Sound sound = sounds.Find(s => s.name == soundName);
        if (sound != null)
        {
            sound.source.Stop();
        }
    }

    public void SetVolume(string soundName, float volume)
    {
        Sound sound = sounds.Find(s => s.name == soundName);
        if (sound != null)
        {
            sound.source.volume = Mathf.Clamp01(volume);
            sound.volume = sound.source.volume; // Update the serialized value
        }
    }

    public void SetPitch(string soundName, float pitch)
    {
        Sound sound = sounds.Find(s => s.name == soundName);
        if (sound != null)
        {
            sound.source.pitch = Mathf.Clamp(pitch, 0.1f, 3f); // Limit pitch to a reasonable range
            sound.pitch = sound.source.pitch; // Update the serialized value
        }
    }

    public void PlayMusic(string soundName)
    {
        StopAllMusic();
        Play(soundName);
    }

    public void StopAllMusic()
    {
        foreach (Sound sound in sounds)
        {
            if (sound.loop)
            {
                sound.source.Stop();
            }
        }
    }
}
