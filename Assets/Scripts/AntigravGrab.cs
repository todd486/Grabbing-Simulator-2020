using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class AntigravGrab : MonoBehaviour {
    public Material defaultMaterial;
    public Material highlightMaterial;

    private GameObject hitObject;

    void FixedUpdate() {
        RaycastHit hit;


        if (hitObject != null) {
            if (hitObject.tag == "RangeGrabbable") {
                hitObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
        }

        if (Physics.Raycast(gameObject.transform.GetChild(0).position, transform.TransformDirection(new Vector3(0, -1f, 1)), out hit, Mathf.Infinity)) {
            Debug.DrawRay(this.gameObject.transform.GetChild(0).position, transform.TransformDirection(new Vector3(0, -1f, 1)) * hit.distance, Color.yellow);

            hitObject = hit.collider.gameObject;

            if (hitObject.tag == "RangeGrabbable") {
                hitObject.GetComponent<MeshRenderer>().material = highlightMaterial;
                Debug.Log(hitObject.name);
            }

        } else {
            Debug.DrawRay(gameObject.transform.GetChild(0).position, transform.TransformDirection(new Vector3(0, -1f, 1)) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
        


    }
}
