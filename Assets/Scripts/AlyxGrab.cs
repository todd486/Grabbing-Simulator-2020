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

    //The apex of the arc which the object is moving towards
    private float handHeight;

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

                    //selectedObject.GetComponent<Rigidbody>().AddForce(calcBallisticVelocityVector(this.transform.position, selectedObject.transform.position, 45) * 3f);
                    //selectedObject.GetComponent<Rigidbody>().AddForce((this.transform.position - selectedObject.transform.position).normalized * (Vector3.Distance(this.transform.position, selectedObject.transform.position) + 3f));

                    selectedObject.GetComponent<Rigidbody>().velocity = calcBallisticVelocityVector(selectedObject.transform.position, this.transform.position);
                }
            }
        }
    }

    //This like actually works ((sometimes)). 
    //Sometimes it fails to calculate the angle at which it should launch it self at, sometimes it just returns NaN for no reason.
    //Most importantly I need to fix the amount of thrust it applies, since it's way too high when close up.
    private Vector3 calcBallisticVelocityVector(Vector3 source, Vector3 target) { //https://answers.unity.com/questions/1362266/calculate-force-needed-to-reach-certain-point-addf-1.html
        Vector3 direction = target - source;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;


        float a = Vector3.Angle(target, source) * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        // calculate velocity
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
    }

    private void Update() {

    }

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
