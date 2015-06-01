using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {

    private float angleSight;
    private float objectivePosition;
    private float mouseSensitivityX;
    private float mouseSensitivityY;

    private float distanceMin;
    private float distance;
    private float distanceMax;

    private Vector3 previousPosition;

    public Vector3 centre;

	// Use this for initialization
	void Start () {
        distance = 100;
        distanceMin = 100;
        distanceMax = 500;

        centre = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
        objectivePosition += Input.GetAxis("Mouse X") * mouseSensitivityX;
        angleSight -= Input.GetAxis("Mouse Y") * mouseSensitivityY;

        if (Input.GetAxis("Mouse ScrollWheel") > 0) distance += 10;
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) distance -= 10;

        //clamps
        angleSight = Mathf.Clamp(angleSight, 1, 100);

        this.transform.rotation = Quaternion.Euler(angleSight, objectivePosition, 0.0f);
        
        previousPosition = this.transform.position - centre;
        this.transform.position = centre + transform.rotation * Vector3.forward * distance;


	}
}
