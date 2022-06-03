using UnityEngine.Audio;
using UnityEngine;

//Reference for Sound Script: https://www.youtube.com/watch?v=6OT43pvUyfY

/*
Definds the format in which sound items can be added in a list format to the
AudioManager and other editable values to be stored as default.
*/
[System.Serializable]
public class Sound {

  public string name;

  public AudioClip clip;

  [Range(0.1f, 3f)]
  public float pitch = 1.0f;

  [HideInInspector]
  public AudioSource source;

}
