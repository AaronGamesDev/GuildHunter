using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCharacter : MonoBehaviour {

    private Vector3 mousePrevPos = Vector3.zero;
    private Vector3 mousePosDelta = Vector3.zero;
    public Camera uiCamera;

    private Ray ray;
    private RaycastHit hit;
    public bool onCharacter = false;
    public bool canRotate = false;

    void Start()
    {
        uiCamera = GameObject.Find("UI Camera").GetComponent <Camera>();
    }
	// Update is called once per frame
	void Update () {

        ray = uiCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(uiCamera.transform.position, ray.direction);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Character")
            {
                onCharacter = true;
            }
            else
            {
                onCharacter = false;
            }
        }
        else
        {
            onCharacter = false;
        }

		if (Input.GetMouseButton(0) && onCharacter)
        {
            canRotate = true;
            
        }
        else if (Input.GetMouseButtonUp(0))
        {
            canRotate = false;
        }

        if (canRotate)
        {
            mousePosDelta = Input.mousePosition - mousePrevPos;
            transform.Rotate(transform.up, -Vector3.Dot(mousePosDelta, Camera.main.transform.right), Space.World);
        }
        mousePrevPos = Input.mousePosition;

	}
}
