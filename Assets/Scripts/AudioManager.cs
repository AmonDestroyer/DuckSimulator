using UnityEngine.Audio;
using System;
using UnityEngine;

//Reference for Audio Manager: https://www.youtube.com/watch?v=6OT43pvUyfY

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public Sound[] backgroundMusic;

    //Initialization
    void Awake()
    {
        foreach (Sound s in sounds)
        {
          s.source = gameObject.AddComponent<AudioSource>();
          s.source.clip = s.clip;
        }

        foreach (Sound bgm in backgroundMusic)
        {
          bgm.source = gameObject.AddComponent<AudioSource>();
          bgm.source.clip = bgm.clip;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
}
