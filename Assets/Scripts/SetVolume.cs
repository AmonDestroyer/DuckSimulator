using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour // Attach to any persistent
{
    public GameObject VolumeSliderGO; // drag and drop volume slider from menu into this box!
    private Slider m_VolumeSlider; //
    private AudioSource[] m_Sounds;

    void Start()
    {
        m_VolumeSlider = VolumeSliderGO.GetComponent<Slider>();
        // SET THE VOLUME SLIDERS VALUE TO SAVED VALUE IN PlayerPrefs class
        m_VolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f); // 0.75f parameter is default value in case
        //The player has not set the "MasterVolume" in PlayerPrefs yet
        //Adds a listener to the main slider and invokes a method when the value changes.


        // GET ALL AUDIO SOURCES FROM THIS SCENE
        // below code taken from a helpful Unity thread
        // https://forum.unity.com/threads/find-all-gameobjects-in-a-scene-by-tag-and-access-the-audio-source-on-them.445374/
        var temp = GameObject.FindGameObjectsWithTag("SoundSource");
        List<AudioSource> m_SoundsList = new List<AudioSource>();

        for (int i = 0; i < temp.Length; i++) {
            foreach(AudioSource audioS in temp[i].GetComponents<AudioSource>())
            {
              m_SoundsList.Add(audioS);
            }
        }
        m_Sounds = m_SoundsList.ToArray();
        // START BY SETTING AudioSource VOLUMES TO WHATEVER'S BEEN SAVED IN PlayerPrefs or Default value
        SetSourceVolumes(PlayerPrefs.GetFloat("MasterVolume", 0.75f));

        // Start listening for changes to volume slider, call ValueChangeCheck() if slider value changes
        // i.e. someone slides it
        m_VolumeSlider.onValueChanged.AddListener (delegate {ValueChangeCheck ();});
    }

    void SetSourceVolumes(float volumeFactor){
        for (int i = 0; i < m_Sounds.Length; i++) {
            m_Sounds[i].volume = volumeFactor;
        }
    }

    void ValueChangeCheck(){
        Debug.Log($"Chanding value {m_VolumeSlider.value}");
        PlayerPrefs.SetFloat("MasterVolume", m_VolumeSlider.value);
        SetSourceVolumes(PlayerPrefs.GetFloat("MasterVolume", 0.75f));
    }


}
