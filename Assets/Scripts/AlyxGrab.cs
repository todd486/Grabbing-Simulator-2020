using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class AlyxGrab : MonoBehaviour {
    public SteamVR_Action_Boolean grabAction;
    public Hand hand;
    public Material highlightMaterial;


    private GameObject selectedObject;
    private GameObject hitObject;
    private Material lastHighlightedMaterial;

    private void OnEnable() {
        if (hand == null) { hand = this.GetComponent<Hand>(); }

        if (grabAction == null) {
            Debug.LogError("<b>[SteamVR Interaction]</b> No grab action assigned!", this);
            return;
        }

        grabAction.AddOnChangeListener(OnGrabActionChange, hand.handType);
    }

    private void OnDisable() {
        if (grabAction != null) {
            grabAction.RemoveOnChangeListener(OnGrabActionChange, hand.handType);
        }
    }

    //Whenever a new SteamVR action occurs
    private void OnGrabActionChange(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue) {
        if (newValue) {
            //Debug.Log("Firing ray");

            if (Physics.Raycast(
                this.gameObject.transform.Find("RayOrigin").position, //The ray origin's position
                this.transform.TransformDirection(new Vector3(0, -1f, 1)), //Rotated to account for controller hand offsets
                out RaycastHit hit, //Variable out
                10f //Max distance
                )
            ) {
                if (hit.collider.gameObject.CompareTag("RangeGrabbable")) { //Check if the hit object is RangeGrabbable and highlight it to the user.
                    selectedObject = hit.collider.gameObject;

                    // think of it as top-down view of vectors: 
                    //   we don't care about the y-component(height) of the initial and target position.
                    Vector3 projectileXZPos = new Vector3(selectedObject.transform.position.x, 0.0f, selectedObject.transform.position.z);
                    Vector3 targetXZPos = new Vector3(transform.position.x, 0.0f, transform.position.z);

                    // rotate the object to face the target
                    selectedObject.transform.LookAt(targetXZPos);

                    // shorthands for the formula
                    float R = Vector3.Distance(projectileXZPos, targetXZPos);
                    float G = Physics.gravity.y;
                    float tanAlpha = Mathf.Tan(70f * Mathf.Deg2Rad);
                    float H = selectedObject.transform.position.y - transform.position.y;

                    // calculate the local space components of the velocity 
                    // required to land the projectile on the target object 
                    float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
                    float Vy = tanAlpha * Vz;

                    // create the velocity vector in local space and get it in global space
                    Vector3 localVelocity = new Vector3(0f, Vy, Vz);
                    Vector3 globalVelocity = selectedObject.transform.TransformDirection(localVelocity);

                    // launch the object by setting its initial velocity and flipping its state
                    selectedObject.GetComponent<Rigidbody>().velocity = globalVelocity;
                }
            }
        }
    }

    

    //This like actually works ((sometimes)). 
    //Sometimes it fails to calculate the angle at which it should launch it self at, sometimes it just returns NaN for no reason.
    //Most importantly I need to fix the amount of thrust it applies, since it's way too high when close up.
    //private Vector3 calcBallisticVelocityVector(Vector3 source, Vector3 target, float mass) { //https://answers.unity.com/questions/1362266/calculate-force-needed-to-reach-certain-point-addf-1.html
    //    //Get direction
    //    Vector3 direction = target - source;
    //    //Get height
    //    float height = direction.y;

    //    //Get total distance in 2D space
    //    direction.y = 0;
    //    float distance = direction.magnitude;

    //    //Get angle and convert angle from degrees to radians
    //    float angle = Vector3.Angle(target, source) * Mathf.Deg2Rad; 

    //    //Set Y-direction to distance
    //    direction.y = distance * Mathf.Tan(angle);
    //    distance += height / Mathf.Tan(angle);

    //    // calculate velocity
    //    float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * angle));
    //    return velocity * direction.normalized;
    //}

  

    //Using FixedUpdate() because apparently you're supposed to do that when raycasting each frame.
    private void FixedUpdate() {
        //Draw debug stuff in scene view
        Debug.DrawRay(this.gameObject.transform.Find("RayOrigin").position, transform.TransformDirection(new Vector3(0, -1f, 1)) * 1000, Color.magenta);

        //Reset last hit object's material
        if (hitObject != null) {
            hitObject.GetComponent<Renderer>().material = lastHighlightedMaterial;
            hitObject = null;
        }

        //Check for highlightable stuff
        if (Physics.Raycast(
                this.gameObject.transform.Find("RayOrigin").position, //The ray origin's position
                transform.TransformDirection(new Vector3(0, -1f, 1)), //Rotated to account for controller hand offsets
                out RaycastHit hit, //Variable out
                10f //Max distance
                )
            ) {

            if (hit.collider.gameObject.CompareTag("RangeGrabbable")) { //Check if the hit object is RangeGrabbable and highlight it to the user.
                //Debug.Log("Is grabbable!");

                hitObject = hit.collider.gameObject;

                lastHighlightedMaterial = hitObject.GetComponent<Renderer>().material;

                hitObject.GetComponent<Renderer>().material = highlightMaterial;
            }
        }
    }
}
