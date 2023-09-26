using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHit : MonoBehaviour
{
    public bool hitOther, leftWorld;
    public Collider other;

    private void Start()
    {
        hitOther = false;
        leftWorld = false;
    }

    public delegate void EventHandler();
    public event EventHandler CollisionDetected;

    public void OnTriggerEnter(Collider other)
    {
        this.other = other;
        if (other.tag == "Enemy")
        {
            Debug.Log("Arrow collided with " + other.tag);
            if (CollisionDetected != null)
            {
                CollisionDetected();
            }
            else
            {
                Debug.Log("Event is Null");
            }
        }
        else if (other.tag != "Enemy" && other.tag != "Player" && other.tag != "Untagged" && other.tag != "World" && other.tag != "Friendly")//ignore these because hitDetect references hitOther and will unsubcribe the event before it destroys the arrow
        {
            hitOther = true;
        }

    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "World")
        {
            Debug.Log("arrow left world");
            leftWorld = true;
        }
    }
}


