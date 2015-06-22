using UnityEngine;
using System.Collections;

public class Camara : MonoBehaviour {

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
    private Vector3 previousPosition;
    private Vector3 actualPosition;

    private RaycastHit impact;
    private CharacterController controller;


	// Use this for initialization
	void Start () {
        distance = 100;
        distanceMin = 0.1f;
        distanceMax = 250;
        interpolatedDistance = distance;

        centre = new Vector3(0, 0, 0);
        
        mouseSensitivityX = 2.5f;
        mouseSensitivityY = 2.5f;

        controller = transform.GetComponent<CharacterController>();

    }
	
	// Update is called once per frame
	void Update () {

        //input
        if (Input.GetKey(KeyCode.Mouse0))
        {
            objectivePosition += Input.GetAxis("Mouse X") * mouseSensitivityX;
            angleSight -= Input.GetAxis("Mouse Y") * mouseSensitivityY;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) distance -= 10;
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) distance += 10;

        //clamps
        angleSight = Mathf.Clamp(angleSight, 5, 80);
        distance = Mathf.Clamp(distance, distanceMin, distanceMax);


        //calculs varis
        previousPosition = transform.position;
        Physics.Raycast(transform.position - transform.forward * distanceMax, transform.forward, out impact, 2*distanceMax);
        float impactToCenterDistance = Vector3.Distance(centre, impact.point);
        relativeDistance = impactToCenterDistance + distance;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(angleSight, objectivePosition, 0.0f), 0.1f);
        interpolatedDistance = Mathf.Lerp(interpolatedDistance, relativeDistance, 0.05f);
        actualPosition = centre + transform.rotation * Vector3.forward * -interpolatedDistance;
        controller.Move(actualPosition - previousPosition);
	}
}
