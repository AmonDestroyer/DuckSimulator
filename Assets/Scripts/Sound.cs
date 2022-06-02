using UnityEngine.Audio;
using UnityEngine;

//Reference for Sound Script: https://www.youtube.com/watch?v=6OT43pvUyfY

[System.Serializable]
public class Sound {

  public string name;

  public AudioClip clip;

  public float volume;
  [Range(0.1f, 3f)]
  public float pitch;

  [HideInInspector]
  public AudioSource source;

}
