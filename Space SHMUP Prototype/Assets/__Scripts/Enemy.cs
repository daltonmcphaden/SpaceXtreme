using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Speed = 0.1f;
    public float fireRate = 0.3f;
    public float health; // base enemy moves slowly and down in a straight line, requires 5 hits
    public int score;

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

        //check to see if projectile is active and if its not return
        if (!otherGO.activeSelf)
            return;

        //make sure the game object it colided with is a projectile
        switch (otherGO.gameObject.tag) {

            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                if (!bndCheck.isOnScreen){
                    otherGO.SetActive(false);
                    Destroy(otherGO);
                    break;
                }
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if (health <=0){
                    this.gameObject.SetActive(false);
                    Destroy(this.gameObject); //Destroy this enemy
                    Score.AddScore(this.score);
                }
                //set projectile to inactive to remove bounce effect from the collision
                otherGO.SetActive(false);
                Destroy(otherGO);
                break;

            default:
                print("Enemy hit by non-{ProjectileHero: " + otherGO.name);
                break;
        }
    }
}
