using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Set in Inspector")]
    public float lifeTime = 5; //lifetime of the ship

    [Header("Set Dynamically: Enemy_2")]
    public Vector3 [] points; // because the ship moves in the shape of a curve, coordinate points are needed as reference

    public float birthTime; //time of ship creation

    // Start is called before the first frame update
    void Start()
    {
        points = new Vector3[3]; //coordinates reference above
        points[0] = pos;
        //setting min and max x for start position
        float xMin = -bndCheck.camWidth + bndCheck.radius;
        float xMax = bndCheck.camWidth - bndCheck.radius;

        Vector3 v;
        v = Vector3.zero;
        v.x = Random.Range(xMin, xMax); //gets the x value for the spawn position randomly within the range of cam x
        v.y = -bndCheck.camHeight * Random.Range(2f, 1); 
        points[1] = v;
        
        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        points[2] = v; // this block setting the second reference point for the movement

        birthTime = Time.time;
    }

    // This enemy moves in a sinusoidal path
    public override void Move()
    {
        
        float u = (Time.time - birthTime) / lifeTime;

        if (u > 1) // destroys the enemy after a specific period of time, which is set to be after 2 movement cycles
        {
            Destroy(this.gameObject);
            Main.enemysLeft--;
            return;
        }
        Vector3 p01, p12;
        u = u - 0.2f*Mathf.Sin(u*Mathf.PI*2); //this is the block for the movement equation
        p01 = (1-u)*points[0] + u*points[1];
        p12 = (1-u)*points[1] + u*points[2];
        pos = (1-u)*p01 + u*p12;
    }

    // Update is called once per frame
    void Update()
    {
       Move(); //calls the overriden move method
    }
}
