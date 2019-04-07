using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this enemy is an asteroid that flys through space untill it gets within 
//a certain distance of the hero ship then becomes a homing asteroid
public class Enemy_3 : Enemy
{
    public Rigidbody rBody;
    public Transform target;
    public float force, rotationForce, distance, triggerDist;
    private bool trigger=false;
   
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();

        //make sure there is a hero object
        if (Hero.S != null)
        {
            //get target which is the hero ship
            target = GameObject.FindGameObjectWithTag("Hero").transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null) //make sure hero ship isnt destroyed
        {
            //distance between the asteroid and the hero ship
            distance = Vector3.Distance(rBody.position, target.position);

            //see if the hero ship has moved within trigger distance of the asteroid
            if (distance < triggerDist) trigger = true;

            //seperate if statement so if the ship moves out of trigger distance the rock will keep following
            if (!trigger)
            {
                //if the ship has not entered trigger distance move down the screen
                base.Move();
            }
            else
            {
                Move();//once ship is inside trigger distance the asteroid will follow it
            }
        }
    }
    private void Update()
    {
        //bounds checking, if the enemy goes off screen it gets destroyed
        if (bndCheck != null && bndCheck.offDown)
        {
            if (pos.y < bndCheck.camHeight - bndCheck.radius)
            {
                Main.enemyList.Remove(gameObject);//remove from enemy list
                Main._enemysLeft--;
                Destroy(gameObject);
            }
        }
        if (bndCheck.offLeft || bndCheck.offRight) //this enemy should never go off the screen to the left or right
        {
            Main.enemyList.Remove(gameObject);//remove from enemy list
            Main._enemysLeft--;
            Destroy(gameObject);
        }
    }

    public override void Move()
    {
        Vector3 direction = target.position - rBody.position; //direction asteroid needs to move in
        direction.Normalize(); // normalises vector to give it a length of 1
        Vector3 rotationAmount = Vector3.Cross(transform.right, direction); //amount rock needs to rotate to head towards ship
        rBody.angularVelocity = rotationAmount * rotationForce; //how quickly the asteroid can change direction
        rBody.velocity = transform.right * force; //how fast it moves
    }
}
