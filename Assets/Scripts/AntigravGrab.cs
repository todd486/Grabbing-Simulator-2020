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
        RaycastHit hit;

        if (drop[source].stateDown && selectedObject != null) { //Drop selected item
            selectedObject.GetComponent<Rigidbody>().useGravity = true;
            selectedObject = null;
        }
        else if (selectedObject != null) { //Grab selected item
            selectedObject.GetComponent<Rigidbody>().useGravity = false;
            selectedObject.transform.position = this.gameObject.transform.GetChild(1).position;
        }

        if (hitObject != null) { //Reset previously hit object material
            if (hitObject.tag == "RangeGrabbable") {
                hitObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
        }

        if (Physics.Raycast(this.gameObject.transform.GetChild(0).position, transform.TransformDirection(new Vector3(0, -1f, 1)), out hit, Mathf.Infinity)) {
            Debug.DrawRay(this.gameObject.transform.GetChild(0).position, transform.TransformDirection(new Vector3(0, -1f, 1)) * hit.distance, Color.yellow);

            hitObject = hit.collider.gameObject;

            if (hitObject.tag == "RangeGrabbable") { //Check if the hit object is valid
                hitObject.GetComponent<MeshRenderer>().material = highlightMaterial;

                if (select[source].stateDown) {
                    Debug.Log("Grabbing!");

                    selectedObject = hitObject;
                }
            }

        } else {
            Debug.DrawRay(this.gameObject.transform.GetChild(0).position, transform.TransformDirection(new Vector3(0, -1f, 1)) * 1000, Color.white);
        }
    }
}
