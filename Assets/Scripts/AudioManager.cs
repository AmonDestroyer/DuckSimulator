using UnityEngine.Audio;
using System;
using UnityEngine;

//Reference for Audio Manager: https://www.youtube.com/watch?v=6OT43pvUyfY

/*
Manages general audio
*/
public class AudioManager : MonoBehaviour
{
    //Create the various lists to add audio
    public Sound[] sounds;
    public Sound[] backgroundMusic;
    public Sound[] loopingSounds;

    /*
    Initialization
    Creates an audio source object for each list and sets the appropriate
    settings
    */
    void Awake()
    {
        foreach (Sound s in sounds)
        {
          s.source = gameObject.AddComponent<AudioSource>();
          s.source.clip = s.clip;
          s.source.pitch = s.pitch;
        }

        foreach (Sound bgm in backgroundMusic)
        {
          bgm.source = gameObject.AddComponent<AudioSource>();
          bgm.source.clip = bgm.clip;
          bgm.source.pitch = bgm.pitch;
        }

        foreach (Sound ls in loopingSounds)
        {
          ls.source = gameObject.AddComponent<AudioSource>();
          ls.source.clip = ls.clip;
          ls.source.pitch = ls.pitch;
          ls.source.loop = true;
        }
    }

    /*
    Returns the sound object of any sound
    */
    private Sound getSound(string name)
    {
      Sound s = Array.Find(sounds, sound => sound.name == name);
      if (s == null)
        s = Array.Find(loopingSounds, loopingSounds => loopingSounds.name == name);
      if (s == null)
        s = Array.Find(backgroundMusic, backgroundMusic => backgroundMusic.name == name);
      return s;
    }

    /*
    Single play of a sound in the sounds list
    */
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    /*
    Play a sound in the looping list
    */
    public void LoopPlay(string name)
    {
        Sound ls = Array.Find(loopingSounds, loopingSounds => loopingSounds.name == name);
        if (!ls.source.isPlaying)
          ls.source.Play();
    }

    /*
    Changes pitch of any sound to pitch value, does not restore pitch value
    after it is changed.
    */
    public void ChangePitch(string name, float pitch)
    {
      Sound s = getSound(name);
      s.source.pitch = pitch;
    }

    /*
    Stops any sound of given name
    */
    public void Stop(string name)
    {
      Sound s = getSound(name);
      if (s != null)
        s.source.Stop();
    }
}
