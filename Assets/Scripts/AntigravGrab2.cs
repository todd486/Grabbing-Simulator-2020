using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class AntigravGrab2 : MonoBehaviour { //Most of this ripped straight from Valve.VR.InteractionSystem.Sample.Planting
    public SteamVR_Action_Boolean grabAction;
    public Hand hand;
    public Material highlightMaterial;

    private GameObject selectedObject;
    private GameObject hitObject;
    private Material lastHighlightedMaterial;
    private bool isGrabbedManually;

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
                transform.TransformDirection(new Vector3(0, -1f, 1)), //Rotated to account for controller hand offsets
                out RaycastHit hit, //Variable out
                10f //Max distance
                )
            ) {
                if (hit.collider.gameObject.CompareTag("RangeGrabbable")) { //Check if the hit object is RangeGrabbable and highlight it to the user.
                    selectedObject = hit.collider.gameObject;

                    selectedObject.GetComponent<Rigidbody>().useGravity = false;

                    UnityEvent AttachEvent = new UnityEvent();
                    AttachEvent.AddListener(OnAttachedToHand);

                    selectedObject.GetComponent<InteractableHoverEvents>().onAttachedToHand = AttachEvent; 
                    
                    //Create and add a new UnityEvent to the RangeGrabbable object which keeps track if the object has been grabbed manually.

                    //TODO: Fix structure so we can remove the listener once we're done. But it works for now, garbage collector be damned.
                }
            }
        } 
    }

    public void OnAttachedToHand() {
        isGrabbedManually = true;
        selectedObject.GetComponent<Rigidbody>().useGravity = true; //TODO: fix gravity not reapplying, NullReferenceException. Do we even need to disable that?
    }

    private void Update() { 
        if (isGrabbedManually) {
            selectedObject = null;
            isGrabbedManually = false; //Reset stuff
        }

        if (selectedObject != null) { //Follow hand
            selectedObject.transform.position = this.gameObject.transform.Find("ObjectAttachmentPoint").position;
        }
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
