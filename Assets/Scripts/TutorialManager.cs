using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    // Public Variables
    public TextMeshProUGUI locationText;
    public float fadeDuration = 1f; //Controls fade in and out duration.
    public float holdDuration = 5f; //Time to hold starting at fade in.
    public CanvasGroup black;

    // Private Variables
    private PlayerController player;
    private float m_Timer = 0f;
    private float m_CompletionTimer = 0f;
    private bool m_RemoveText=false;
    private bool m_EnableText=false;
    private bool m_Completed=false;
    private bool m_fadeIn = true;

    // Start is called before the first frame update
    void Start()
    {
      player = GetComponent<PlayerController>();
      player.jumpNum = 0;
      player.sprintSpeed = player.walkSpeed;

      locationText.text = "Use WASD to move" + "\n" + "Use Mouse to look";
    }

    // Update is called once per frame
    void Update()
    {
      if(m_fadeIn)
      {
        m_Timer += Time.deltaTime;
        black.alpha = 1 - m_Timer/fadeDuration;
        if (m_Timer > fadeDuration)
          m_fadeIn = false;
      }

      if (m_RemoveText)
      {
        DisableText();
      }
      else if (m_EnableText){
        EnableText();
      }
      if (m_Completed){
        m_CompletionTimer += Time.deltaTime;
        if (m_CompletionTimer > holdDuration)
        {
          black.alpha = (m_CompletionTimer-holdDuration)/fadeDuration;
        }
        if (m_CompletionTimer > (holdDuration + fadeDuration)){
          Debug.Log("Scene Completed");
          SceneManager.LoadScene("MainScene");
        }
      }
    }

    //Text Modifications
    void DisableText(){
      m_Timer += Time.deltaTime;

      if (m_Timer < fadeDuration)
      {
        locationText.alpha = 1 - m_Timer/fadeDuration ;
      }
      else
      {
        m_RemoveText = false;
      }
    }

    void EnableText()
    {
      m_Timer += Time.deltaTime;

      locationText.alpha = m_Timer / fadeDuration;
      if (m_Timer > holdDuration)
      {

        m_Timer = 0f;
        m_EnableText = false;
        m_RemoveText = true;
      }
    }

    void SetEnableVariables()
    {
      m_Timer = 0f;
      m_EnableText = true;
    }

    //Waypoints that enable various tasks, but don't
    void DisableMoveText()
    {
      Debug.Log("Disable Movement Text");
      m_Timer = 0f;
      m_RemoveText = true;
    }

    // Functions that are done at various checkpoints for the tutorial
    void EnableJump()
    {
      player.jumpNum = 3;
      locationText.text = "Feathers" + "\n" +
      "You have collected a feather, feathers can be used to unlock/upgrade abilities. " +
      "Here the jump has been unlocked.";
      SetEnableVariables();
    }

    void EnableMelee()
    {
      Debug.Log("Enabling Melee");
      locationText.text = "Melee " + "\n" +
      "A melee can be performed by looking at an enemy and pressing 'v' ";
      SetEnableVariables();
    }

    void EnableSprint()
    {
      player.sprintSpeed *= 2;
      locationText.text = "Sprinting" + "\n" +
      "Use SHIFT to sprint so you can dodge.";
      SetEnableVariables();
    }

    void EnableBow()
    {
      Debug.Log("Enable Bow");
      locationText.text = "Bow" + "\n" +
      "The is a weapon that can be used to fire arrows at enemies using " +
      "<insert button here>.";
      SetEnableVariables();
    }

    void StageCompletion()
    {
      locationText.text = "Tutorial Completed!" + "\n" +
      "You have safely made it back to your raft!";
      m_Completed = true;
      SetEnableVariables();
    }

    void SecretAreaFound()
    {
      Debug.Log("SecretAreaFound");
      locationText.text = "Secret Area!" + "\n" +
      "Find more in future levels!";
      SetEnableVariables();
    }

    void OnCollisionEnter(Collision other)
    {
      if(other.gameObject.CompareTag("waypoint")){
        other.gameObject.SetActive(false);
      }
      switch(other.gameObject.name)
      {
        case "MovementWaypoint":
          DisableMoveText();
          break;
        case "SecretArea":
          other.gameObject.SetActive(false);
          SecretAreaFound();
          break;
        case "JumpWaypoint":
          EnableJump();
          break;
        case "MeleeCheckpoint":
          EnableMelee();
          break;
        case "SprintCheckpoint":
          EnableSprint();
          break;
        case "PrebossCheckpoint":
          EnableBow();
          break;
        case "CompletionWaypoint":
          StageCompletion();
          break;
        default:
          Debug.Log("Unknown collision");
          break;
      }
    }


}
