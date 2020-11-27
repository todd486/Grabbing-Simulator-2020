using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
    Vector3 startPos = new Vector3(0, 0, 0);
    Vector3 endPos = new Vector3(0, 0, 10);
    float height = 4f;
    bool startThrow = false;
    float incrementor = 0;

    // Update is called once per frame
    void Update() {
        if (startThrow) {
            incrementor += 0.04f;
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, incrementor);
            currentPos.y += height * Mathf.Sin(Mathf.Clamp01(incrementor) * Mathf.PI);
            transform.position = currentPos;
        }
        if (transform.position == endPos) {
            startThrow = false;
            incrementor = 0;
            Vector3 tempPos = startPos;
            startPos = transform.position;
            endPos = tempPos;
        }
        if (Input.GetMouseButtonDown(0)) {
            startThrow = true;
        }
    }
}