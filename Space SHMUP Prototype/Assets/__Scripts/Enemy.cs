using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Speed = 0.1f;
    public float fireRate = 0.3f;
    public float health = 10; // base enemy moves slowly and down in a straight line, requires 5 hits
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
        switch (otherGO.gameObject.tag) {

            case "ProjectileHero":
            Projectile p = otherGO.GetComponent<Projectile>();
            if (!bndCheck.isOnScreen){
                Destroy(otherGO);
                break;
            }
            health -= Main.GetWeaponDefinition(p.type).damageOnHit;
            if (health <=0){
                Destroy(this.gameObject); //Destroy this enemy
            }
            Destroy(otherGO);
            break;

            default:
            print("Enemy hit by non-{ProjectileHero: " + otherGO.name);
            break;
        }
    }
}
