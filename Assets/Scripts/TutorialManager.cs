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
    private GameObject hunter;

    // Private Variables
    private PlayerController m_playerController;
    private EnemyBase m_HunterScript;
    private float m_CompletionTimer = 0f;
    private float m_Timer = 0f;
    private bool m_justSpawned = true;
    private bool m_RemoveText=false;
    private bool m_EnableText=false;
    private bool m_Completed=false;

    // Scene Manager
    private AnySceneManager m_AnySceneManager;

    void OnEnable()
    {
      m_AnySceneManager = GameObject.FindGameObjectWithTag("sceneManager").GetComponentInChildren(typeof(AnySceneManager)) as AnySceneManager;

      if(m_AnySceneManager == null) { // try again, but with inactive objects too
        m_AnySceneManager = GameObject.FindGameObjectWithTag("sceneManager").GetComponentInChildren(typeof(AnySceneManager), true) as AnySceneManager;
      }
      //MainManager = FindObjectOfType<NeverUnloadSceneManager>();
      
      //Player initial variables
      m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
      m_playerController.jumpNum = 0;
      m_playerController.enableFire = false;
      m_playerController.sprintSpeed = m_playerController.walkSpeed;
      m_playerController.spawnPoint = GameObject.Find("StartSpawnPoint").transform;
      m_playerController.Respawn();
      m_playerController.stdTime();
      
      //For enabling/disabling hunter's shooting
      hunter = GameObject.Find("DuckHunter");
      m_HunterScript = hunter.GetComponentInChildren<EnemyBase>() as EnemyBase;
      //Set Instructions
      locationText = GameObject.Find("InstructionText").GetComponentInChildren<TextMeshProUGUI>();
      locationText.text = "Use WASD to move" + "\n" + "Use Mouse to look";
    }

    // Update is called once per frame
    void Update()
    {

      if (m_RemoveText)
      {
        DisableText();
      }
      else if (m_EnableText){
        EnableText();
      }

        if (m_CompletionTimer > (holdDuration + fadeDuration)){
          Debug.Log("Scene Completed");
          //MainManager.EndMainScene();
          m_AnySceneManager.TransitionScene(1, 2);;
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
        SetDisableVariables();
        if(m_Completed)
          m_AnySceneManager.TransitionScene(3, 2);
      }
    }

    void SetEnableVariables()
    {
      m_Timer = 0f;
      m_EnableText = true;
    }

    void SetDisableVariables()
    {
      m_Timer = 0f;
      m_EnableText = false;
      m_RemoveText = true;
    }

    //Waypoints that enable various tasks, but don't
    void DisableMoveText()
    {
      Debug.Log("Disable Movement Text");
      m_Timer = 0f;
      m_RemoveText = true;
    }

    void EnterStage() {
      locationText.text = "Welcome to Duck Simulator!" + "\n" +
      "To begin, go collect that feather over there." + "\n" +
      "Move using WASD.";
      SetEnableVariables();
    }

    // Functions that are done at various checkpoints for the tutorial
    void EnableJump()
    {
      m_playerController.jumpNum = 4;
      locationText.text = "Feathers" + "\n" +
      "You have collected a feather!" + "\n" +
      "Now you can jump using SPACE, try jumping multiple times";
      SetEnableVariables();
    }

    void EnableMelee()
    {
      Debug.Log("Enabling Melee");
      locationText.text = "Melee " + "\n" +
      "A melee can be performed by looking at an enemy and pressing Right Mouse Button.\n" +
      "When you kill an enemy, they get to go to heaven :)" + "\n" +
      "You also get health if they're close to you when they die.";
      SetEnableVariables();
    }

    void EnableSprint()
    {

      m_playerController.sprintSpeed = 0.4f;
      locationText.text = "Sprinting" + "\n" +
      "Use SHIFT to sprint so you can dodge. \n" +
      "BLUE means the hunter sees you - RED means\n" +
      "he's about to shoot!";
      m_HunterScript.approachRadius = 500f;
      SetEnableVariables();
    }

    void EnableBow()
    {
      m_playerController.enableFire = true;
      locationText.text = "Bow" + "\n" +
      "The is a weapon that can be used to fire arrows at enemies using " +
      "Left Mouse Button.";
      m_HunterScript.approachRadius = 0f;
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
        case "StartSpawnPoint":
          if(m_justSpawned) {
            m_justSpawned = false;
            EnterStage();
          }
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
