using UnityEngine;
using System.Collections;
 
 public class LaserScript : MonoBehaviour
 { 
     // https://answers.unity.com/questions/1142318/making-raycast-line-visibld.html 100% stolen, no adaptation
     public LineRenderer laserLineRenderer;
     public Transform origin;
     public Vector3 target;
     public float laserWidth = 0.1f;
     public float laserMaxLength = 5f;
     public bool triggerLaser = false;
     public Color c1 = Color.black;
     public Color c2 = Color.blue;
 
     void Start() {
         Vector3[] initLaserPositions = new Vector3[  ] { Vector3.zero, Vector3.zero };
         laserLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
         laserLineRenderer.SetPositions( initLaserPositions );
         laserLineRenderer.startWidth = laserWidth;
         laserLineRenderer.endWidth = laserWidth;
         ChangeColor(Color.blue);
     }
 
     public void ChangeColor(Color newColor) {
         c2 = newColor;
         float alpha = 1.0f;
         Gradient gradient = new Gradient();
         gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        laserLineRenderer.colorGradient = gradient;
     }

     void FixedUpdate() 
     {
         if(triggerLaser) {
             ShootLaserFromTargetPosition(origin.position, target - origin.position, laserMaxLength );
             laserLineRenderer.enabled = true;
         }
         else {
             laserLineRenderer.enabled = false;
         }
     }
 
     void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length )
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