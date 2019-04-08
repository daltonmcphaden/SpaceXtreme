using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colour : MonoBehaviour
{
    //Material for Hero ship
    public Material shipMat;
    
    void Start()
    {
        //Initialize as white
        shipMat.color = Color.white;
    }

    //Functions to change to each colour
    public void MakeWhite()
    {
        shipMat.color = Color.white;
    }

    public void MakeGreen()
    {
        shipMat.color = Color.green;
    }
    public void MakeBlue()
    {
        shipMat.color = Color.blue;
    }
    public void MakeRed()
    {
        shipMat.color = Color.red;
    }
}
