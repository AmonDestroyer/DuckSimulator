using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherController : MonoBehaviour
{   
    public GameObject HUD;
    private HUDController hudController;
    
    void Start(){
        //hudController = HUD.GetComponent<HUDController>();
    }

    void Update()
    {
        // ROTATE FEATHER - made using technique 
        // from Unity Rollaball tutorial
        transform.Rotate(new Vector3(15, 60, 20) * Time.deltaTime);  
    }

    // Make feather dissapear when the duck touches it
    // and then update the feather count variable
    private void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("Player")){
            // make feather dissapear
            this.gameObject.SetActive(false);
            // update feather count in HUD
            hudController = HUD.GetComponent<HUDController>();
            hudController.featherCount = hudController.featherCount + 1;
            hudController.SetCountText();
        }
    }
}
