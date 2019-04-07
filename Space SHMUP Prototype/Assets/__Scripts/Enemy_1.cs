using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    private float _x0, _y0; //original positions


   // Start is called before the first frame update
    void Start()
    {
        _x0 = pos.x; // gets 1 original x pos from its starting point
    }

    // Update is called once per frame
    void Update()
    {
        Move(); //calling the function for the ship to move
    }

       public override void Move() //overriding base enemy class move
    {
        //this block senses if the enemy started on the left and right, and moves it in the correct corresponding x direction
        Vector3 tempPos = pos;
        if(_x0 > 0)
        tempPos.x -= speed + Time.deltaTime;
        if (_x0 < 0)
        tempPos.x += speed + Time.deltaTime;

        pos = tempPos;

        // Call to base accounts for the movement in the Y direction
        base.Move();

        //bounds checking, if the enemy goes off screen it gets destroyed
        if (bndCheck != null && bndCheck.offDown || bndCheck.offLeft || bndCheck.offRight) 
        {
            if (pos.y < bndCheck.camHeight - bndCheck.radius)
            {
                Main.enemyList.Remove(gameObject);//remove from enemy list
                Main._enemysLeft--;
                Destroy(gameObject);
            }
        }

    }
}
