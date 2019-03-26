using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsCheck : MonoBehaviour
{
    public float radius = 1f; //This is the Radius
    public bool keepOnScreen = true; // a boolean used for keeping the object on screen

    public bool isOnScreen = true; // a boolean used to check if an object is on screen
    public float camWidth; // width of the camera
    public float camHeight; // height of the camera
    [HideInInspector]
    public bool offRight, offLeft, offUp, offDown; //booleans for where the object is specifically off screen

    private void Awake()
    {
        camHeight = Camera.main.orthographicSize; // sets camHeight to the height of the camera
        camWidth = camHeight * Camera.main.aspect; // sets camWidth to the width of the cam
    }

    // Checks to see if object is out of bounds
    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        isOnScreen = true;
        offRight = offLeft = offDown = offUp = false;

        if (pos.x > camWidth - radius) // if statement checking if off screen to the right
        {
            pos.x = camWidth - radius;
            offRight = true;
        }
        if (pos.x < -camWidth + radius) // if statement checking if off screen to the left
        {
            pos.x = -camWidth +radius;
            offLeft = true;
        }
        if (pos.y > camWidth - radius) // if statement checking if off screen above
        {
            pos.y = camWidth - radius;
            offUp = true;
        }
        if (pos.y < -camWidth + radius) // if statement checking if off screen below
        {
            pos.y = -camWidth + radius;
            offDown = true;
        }

        isOnScreen = !(offRight || offLeft || offUp || offDown); //object is still on screen (default case)
        if (keepOnScreen && !isOnScreen) 
        {
             transform.position = pos;
             offRight = offLeft = offUp = offDown = false;
        }
       
    }

    private void OnDrawGizmos() 
    {
        if (!Application.isPlaying) return;
        Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}
