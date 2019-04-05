using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colour : MonoBehaviour
{
    public Material shipMat;
    
    void Start()
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
