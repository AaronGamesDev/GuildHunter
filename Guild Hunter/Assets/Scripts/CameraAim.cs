using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAim : MonoBehaviour {
    [Range(0.1f, 1f)]
    public float followSpeed = 0.2f;
    [Range(0.1f, 100f)]
    public float rotateSpeed = 5f;
    public Vector3 playerPos, offset;
    public bool inverted = true;

	// Use this for initialization
	void Start () {
        followSpeed = 0.2f;
        offset = transform.position - playerPos;
        inverted = true;
    }
	
	// Update is called once per frame
	void Update () {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        LookAtPlayer();
        MouseRotate();
        FollowPlayer();
        if (offset.y <= 1)
        {
            offset.y = 1;
        }
        else if (offset.y >= 5)
        {
            offset.y = 5;
        }

    }

    void FollowPlayer()
    {
        Vector3 newPos = playerPos + offset;
        transform.position = Vector3.Slerp(transform.position, newPos, followSpeed);
    }

    void LookAtPlayer()
    {
        transform.LookAt(playerPos);
    }

    void MouseRotate()
    {
        if (inverted)
        {
            Quaternion turnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed, Vector3.up) * Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * Time.deltaTime * -rotateSpeed, Vector3.right);
            offset = turnAngle * offset;
        }
        else
        {
            Quaternion turnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed, Vector3.up) * Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * Time.deltaTime * rotateSpeed, Vector3.right);
            offset = turnAngle * offset;
        }

        
    }
}
