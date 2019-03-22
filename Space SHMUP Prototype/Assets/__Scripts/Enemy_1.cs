using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    private float _x0, _y0; 
    

   // Start is called before the first frame update
    void Start()
    {
        _x0 = pos.x;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

       public override void Move()
    {
        Vector3 tempPos = pos;
        if(_x0 > 0)
        tempPos.x -= Speed + Time.deltaTime;
        if (_x0 < 0)
        tempPos.x += Speed + Time.deltaTime;

        pos = tempPos;

        // Call to base accounts for the movement in the Y direction
        base.Move();

        if (bndCheck != null && bndCheck.offDown || bndCheck.offLeft || bndCheck.offRight)
        {
            if (pos.y < bndCheck.camHeight - bndCheck.radius)
            {
                Destroy(gameObject);
            }
        }

    }
}
