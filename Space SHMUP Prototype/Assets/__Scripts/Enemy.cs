using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Speed = 0.1f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;

    protected BoundsCheck bndCheck;

    public Vector3 pos {
        get {
            return(this.transform.position);
        }
        set {
            this.transform.position = value;
        }
    }

    void Awake() {
        bndCheck = GetComponent<BoundsCheck>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (bndCheck != null && bndCheck.offDown) {
            Destroy(gameObject);
        }
    }

    // Moves enemies in the Y direction
    public virtual void Move() {
        Vector3 tempPos = pos;
        tempPos.y -= Speed + Time.deltaTime;
        pos = tempPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherGO = collision.gameObject;

        //make sure the game object it colided with is a projectile
        if (otherGO.tag == "ProjectileHero")
        {
            //destroys the projectile and the enemy if a collision is detected
            Destroy(otherGO);
            Destroy(gameObject);
        }
    }
}
