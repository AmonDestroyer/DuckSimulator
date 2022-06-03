using UnityEngine;
using System.Collections;
 
 public class LaserScript : MonoBehaviour
 { 
     // https://answers.unity.com/questions/1142318/making-raycast-line-visibld.html 100% stolen, no adaptation
     public LineRenderer laserLineRenderer;
     public float laserWidth = 0.1f;
     public float laserMaxLength = 5f;
     public bool triggerLaser = false;
 
     void Start() {
         Vector3[] initLaserPositions = new Vector3[ 2 ] { Vector3.zero, Vector3.zero };
         laserLineRenderer.SetPositions( initLaserPositions );
         laserLineRenderer.startWidth = laserWidth;
         laserLineRenderer.endWidth = laserWidth;
         ChangeColor(Color.red);
     }
 
     public void ChangeColor(Color newColor) {
         laserLineRenderer.startColor = newColor;
         laserLineRenderer.endColor = newColor;
     }
     void Update() 
     {
         if(triggerLaser) {
             ShootLaserFromTargetPosition( transform.position, Vector3.forward, laserMaxLength );
             laserLineRenderer.enabled = true;
         }
         else {
             laserLineRenderer.enabled = false;
         }
     }
 
     void ShootLaserFromTargetPosition( Vector3 targetPosition, Vector3 direction, float length )
     {
         Ray ray = new Ray( targetPosition, direction );
         RaycastHit raycastHit;
         Vector3 endPosition = targetPosition + ( length * direction );
 
         if( Physics.Raycast( ray, out raycastHit, length ) ) {
             endPosition = raycastHit.point;
         }
 
         laserLineRenderer.SetPosition( 0, targetPosition );
         laserLineRenderer.SetPosition( 1, endPosition );
     }
 }