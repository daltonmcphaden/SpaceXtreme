using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType // Enum definition for all the weapons
{
    none, blaster, spread, spray, missile, laser, shield
}

[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; //letter to show on the powerup
    public Color color; // Weapon and its projectile color
    public GameObject projectilePrefab; // projectile game object
    public float damageOnHit, continousDamage, delayBetweenShots, velocity; //weapon fire properties damage, delay, velocity of projectile
    public Transform target;
    public float force, rotationForce;
}
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public GameObject gun;
    public float lastShotTime;
    private Renderer _collarRend;
    private Renderer _gunColor;
    public AudioSource ausource;
    public AudioClip fire;
    public static float _vol = 0.8f;
    public Projectile m; //missile decliration


    // Start is called before the first frame update
    void Start()
    {
        collar = transform.Find("Collar").gameObject;
        gun = transform.Find("Barrel").gameObject;
        _collarRend = collar.GetComponent<Renderer>();
        _gunColor = gun.GetComponent<Renderer>();

        //set the default type
        SetType(_type);

        //dynamicaly create an anchor for all projectiles
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject gameObj = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = gameObj.transform;
        }

        //find the firedelegate of the root game object
        GameObject rootGO = transform.root.gameObject;
        if(rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }
//gets and sets weapon type
    public WeaponType type
    {
        get { return _type; }
        set { SetType(value); }
    }
//sets weapon type and if nothing is passed sets it to default weapon
    public void SetType(WeaponType weaponType)
    {
        _type = weaponType;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }

        def = Main.GetWeaponDefinition(_type);
        _collarRend.material.color = def.color; // sets collar colour depending on weapon in use
        _gunColor.material.color = def.color;   // Set gun color depending on weapon in use
        lastShotTime = 0; 
    }

    public void Fire()
    {
        //if this game object is inactive return
        if (!gameObject.activeInHierarchy)
            return;

        //if there hasnt been long enough between shots return
        if (Time.time - lastShotTime < def.delayBetweenShots)
            return;
        
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;

        if (transform.up.y < 0)
            vel.y = -vel.y;
        
        //switch statement for weapon types
        switch (type) 
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                ausource.PlayOneShot(fire,_vol);
                break;

            case WeaponType.spread:

                switch(Hero.S.weaponLevels[1]) {

                    case 1:

                        // Right projectile
                        p = MakeProjectile();
                        p.transform.rotation = Quaternion.AngleAxis(15, Vector3.back); 
                        p.rigid.velocity = p.transform.rotation * vel;

                        // Center projectile
                        p = MakeProjectile();
                        p.rigid.velocity = vel;

                        // Left projectile
                        p = MakeProjectile();
                        p.transform.rotation = Quaternion.AngleAxis(-15, Vector3.back);
                        p.rigid.velocity = p.transform.rotation * vel;

                        break;

                    case 2:
                        // Right projectile
                        p = MakeProjectile();
                        p.transform.rotation = Quaternion.AngleAxis(30, Vector3.back); 
                        p.rigid.velocity = p.transform.rotation * vel;

                        // Left projectile
                        p = MakeProjectile();
                        p.transform.rotation = Quaternion.AngleAxis(-30, Vector3.back);
                        p.rigid.velocity = p.transform.rotation * vel;

                        goto case 1;

                    case 3:

                        // Right projectile
                        p = MakeProjectile();
                        p.transform.rotation = Quaternion.AngleAxis(45, Vector3.back); 
                        p.rigid.velocity = p.transform.rotation * vel;

                        // Left projectile
                        p = MakeProjectile();
                        p.transform.rotation = Quaternion.AngleAxis(-45, Vector3.back);
                        p.rigid.velocity = p.transform.rotation * vel;

                        goto case 2;

                }
                ausource.PlayOneShot(fire, 0.8f);
                break;

            case WeaponType.spray:   

                p = MakeProjectile();
                p.transform.rotation = gun.transform.rotation;      // Bullets fire in the direction that the gun is pointed in

                p.rigid.velocity = gun.transform.rotation*vel;  

                Destroy(p.gameObject, 0.2f); 
                ausource.PlayOneShot(fire,0.1f);  
                break;

            case WeaponType.missile:
                
                //make sure there is an enemy object
                if (Main.enemyList.Count != 0)
                {
                    m = MakeProjectile();
                    //get closest target 
                    def.target = GetClosestEnemy(Main.enemyList);
                }
                break;
        }
    }
    private void FixedUpdate()
    {
        if (def.target == null || m == null)  //make sure theres a target
            return;
        
        Vector3 direction = def.target.position - m.rigid.position; //direction projectile needs to move in
        direction.Normalize(); // normalises vector to give it a length of 1
        Vector3 rotationAmount = Vector3.Cross(m.transform.up, direction); //amount projectile needs to rotate to track enemy
        m.rigid.angularVelocity = rotationAmount * def.rotationForce; //how quickly the  projectile can change direction
        m.rigid.velocity = m.transform.up * def.force; //how fast it moves
        
    }
    //function that gets the closest enemy to the ship it takes the current list of enemys as its parameter
    Transform GetClosestEnemy(List<GameObject> enemies)
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity; //set closest distance to infinity so it can be compared to smallet distances
        Vector3 currentPosition = Main.S.transform.position; //current position of hero ship

        foreach (GameObject potentialTarget in enemies) //looks at each potential enemy in the enemy list
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition; //get the direction towards the target
            float targetDist = directionToTarget.sqrMagnitude; // the distance to the target

            if (targetDist < closestDistance) //compare the closest distance found so far to the current target distance
            {
                closestDistance = targetDist; //if a closer distance is found it sets it to the new closest distance
                closestTarget = potentialTarget.transform; //and it sets the closest target
            }
        }
        return closestTarget;
    }
    public Projectile MakeProjectile()
    {
        GameObject gameObj = Instantiate<GameObject>(def.projectilePrefab); //makes projectile object from the prefab
        if (transform.parent.gameObject.tag == "Hero") //if projeectile coming from hero, sets projectile tag and layer to projectileHero
        {
            gameObj.tag = "ProjectileHero";
            gameObj.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else //same as above for enemy projactiles
        {
            gameObj.tag = "ProjectileEnemy";
            gameObj.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        gameObj.transform.position = collar.transform.position; // movement of weapon collar
        gameObj.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = gameObj.GetComponent<Projectile>(); // sets a projectile pointer variable to projectile object
        p.type = type; //sets type of pointer to type of projectile
        lastShotTime = Time.time; //sets last shot time to time projectile was shot
        return p; //returns the pointer variable
    }
    
}
