using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class AntigravGrab : MonoBehaviour {
    public SteamVR_Action_Boolean select;
    public SteamVR_Action_Boolean drop;
    public SteamVR_Input_Sources source;


    public Material defaultMaterial;
    public Material highlightMaterial;

    private GameObject hitObject;
    private GameObject selectedObject;


    void FixedUpdate() {

        if (select[source].stateDown && selectedObject != null) { //Drop selected item
            Debug.Log("Dropping!");
            selectedObject.GetComponent<Rigidbody>().useGravity = true;
            selectedObject = null;
        }

        if (hitObject != null) { //Reset previously hit object material
            if (hitObject.CompareTag("RangeGrabbable")) {
                hitObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
        }

        if (Physics.Raycast(
                this.gameObject.transform.GetChild(0).position, 
                transform.TransformDirection(new Vector3(0, -0.75f, 1f)), out RaycastHit hit, 10f)
            ) {
            Debug.DrawRay(
                this.gameObject.transform.GetChild(0).position, 
                transform.TransformDirection(new Vector3(0, -0.75f, 1f)) * hit.distance, Color.yellow);

            hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("RangeGrabbable")) { //Check if the hit object is valid
                hitObject.GetComponent<MeshRenderer>().material = highlightMaterial;

                if (select[source].stateDown) {
                    Debug.Log("Grabbing!");
                    selectedObject = hitObject; //Set the hitObject to the selected object
                    selectedObject.GetComponent<Rigidbody>().useGravity = false;
                }
            }

        } else {
            Debug.DrawRay(this.gameObject.transform.GetChild(0).position, transform.TransformDirection(new Vector3(0, -1f, 1)) * 1000, Color.white);
        }
    }

    private void Update() {
        FollowHand();
    }

    void FollowHand() {
        if (selectedObject != null) { //Follow hand
            selectedObject.transform.position = Vector3.Lerp(selectedObject.transform.position, this.gameObject.transform.GetChild(1).position, 1f);
        }
    }
}
