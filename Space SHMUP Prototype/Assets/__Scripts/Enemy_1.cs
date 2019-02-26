using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    private float x0, y0; 

   // Start is called before the first frame update
    void Start()
    {
        x0 = pos.x;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

       public override void Move()
    {
        Vector3 tempPos = pos;
        if(x0 > 0)
        tempPos.x -= Speed + Time.deltaTime;
        if (x0 < 0)
        tempPos.x += Speed + Time.deltaTime;

        pos = tempPos;

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
