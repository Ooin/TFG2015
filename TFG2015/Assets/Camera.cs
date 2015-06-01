﻿using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {

    private float angleSight;
    private float objectivePosition;
    private float mouseSensitivityX;
    private float mouseSensitivityY;

    private float distanceMin;
    private float distance;
    private float distanceMax;
    private float interpolatedDistance;
    private float relativeDistance;

    public Vector3 centre;

    private RaycastHit impact;


	// Use this for initialization
	void Start () {
        distance = 100;
        distanceMin = 10;
        distanceMax = 250;
        interpolatedDistance = distance;

        centre = new Vector3(-50, 0, 50);
        
        mouseSensitivityX = 2.5f;
        mouseSensitivityY = 2.5f;

    }
	
	// Update is called once per frame
	void Update () {
        //input
        objectivePosition += Input.GetAxis("Mouse X") * mouseSensitivityX;
        angleSight -= Input.GetAxis("Mouse Y") * mouseSensitivityY;

        if (Input.GetAxis("Mouse ScrollWheel") > 0) distance -= 10;
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) distance += 10;

        //clamps
        angleSight = Mathf.Clamp(angleSight, 5, 80);
        distance = Mathf.Clamp(distance, distanceMin, distanceMax);


        //calculs varis
        Physics.Raycast(transform.position, transform.forward, out impact, distanceMax);
        float impactToCenterDistance = Vector3.Distance(centre, impact.point);
        relativeDistance = impactToCenterDistance + distance;

        this.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(angleSight, objectivePosition, 0.0f), 0.1f);
        interpolatedDistance = Mathf.Lerp(interpolatedDistance, relativeDistance, 0.05f);
        this.transform.position = centre + transform.rotation * Vector3.forward * -interpolatedDistance;

	}
}
