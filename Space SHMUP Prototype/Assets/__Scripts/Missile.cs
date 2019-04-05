using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Projectile
{
    public Rigidbody rBody;
    public Transform target;
    public float force, rotationForce;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();

        //make sure there is a hero object
        if (Main.enemyList.Count!=0)
        {
            //get closest target 
            target = GetClosestEnemy(Main.enemyList);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null) //make sure theres a target
        {
            Vector3 direction = target.position - rBody.position; //direction asteroid needs to move in
            direction.Normalize(); // normalises vector to give it a length of 1
            Vector3 rotationAmount = Vector3.Cross(transform.right, direction); //amount rock needs to rotate to head towards ship
            rBody.angularVelocity = rotationAmount * rotationForce; //how quickly the asteroid can change direction
            rBody.velocity = transform.right * force; //how fast it moves
        }
    }

    Transform GetClosestEnemy(List<GameObject> enemies)
    {
        Transform bestTarget = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = Main.S.transform.position;
        foreach (GameObject potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dToTarget = directionToTarget.sqrMagnitude;
            if (dToTarget < closestDistance)
            {
                closestDistance = dToTarget;
                bestTarget = potentialTarget.transform;
            }
        }
        return bestTarget;
    }
}
