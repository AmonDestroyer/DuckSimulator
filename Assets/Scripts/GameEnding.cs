using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnding : MonoBehaviour
{
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;
    public float wonDisplayImageDuration = 10.0f;
    public bool playerDied = false;
    public bool playerWon = false;
    public GameObject player;
    public CanvasGroup deadBackgroundImageCanvasGroup;
    public CanvasGroup wonBackgroundImageCanvasGroup;
    private PlayerController m_PlayerController;

    float m_Timer;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerDied){  //set to true in player controller to activate death!
            Died();
        }
        if(playerWon){  //set to true in player controller to activate death!
            Won();
        }
    }

    void Died(){
        m_Timer += Time.deltaTime;
        deadBackgroundImageCanvasGroup.alpha = m_Timer / fadeDuration;
        if(m_Timer > fadeDuration + displayImageDuration)
        {   
            playerDied = false; //reset for next death
            m_Timer = 0;
            // respawn player at last spawn point and refill health
            m_PlayerController.Respawn();
            m_PlayerController.health = 1.0f;
            //then reset Death Fader
            deadBackgroundImageCanvasGroup.alpha = 0.0f;
           
        }
    }

    void Won(){
        m_Timer += Time.deltaTime;
        wonBackgroundImageCanvasGroup.alpha = m_Timer / fadeDuration;
        if(m_Timer > fadeDuration + wonDisplayImageDuration)
        {   
            Application.Quit();
        }
    }
}
