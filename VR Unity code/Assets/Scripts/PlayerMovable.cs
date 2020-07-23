using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovable : MonoBehaviour
{    
    public GameObject hand;
    public PlayerController player;
    Vector3 grabOffset;

    private bool handInside = false;    

    private void Update() 
    {
        if (handInside && player.isDrawing == false && player.isGrabbing == false)
        {
            if (player.ifButtonADown)
            {
                player.isGrabbing = true;
                transform.position = hand.transform.position;
            }
            else if (player.ifButtonBDown)
            {
                player.isHandInBox = false;
                player.isGrabbing = false;
                player.isDrawing = false;
                Destroy(gameObject);
            }
        }
        else if (player != null)
        {
            player.isGrabbing = false;
        }        
    }

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log(other.transform.name);
        if (other.CompareTag("Hand"))
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
            hand = other.gameObject;
            if (hand != null)
            {
                player = hand.transform.GetComponentInParent<PlayerController>();
            }
            handInside = true;
            player.isHandInBox = handInside;
        }    
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Hand"))
        {
            GetComponent<MeshRenderer>().material.color = Color.white;
            hand = null;
            handInside = false;
            player.isHandInBox = handInside;
        }    
    }
}
