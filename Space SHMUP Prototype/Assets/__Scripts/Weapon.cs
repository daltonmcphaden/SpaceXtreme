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
                
                break;

            case WeaponType.spray:   

                p = MakeProjectile();
                p.transform.rotation = gun.transform.rotation;      // Bullets fire in the direction that the gun is pointed in
                p.rigid.velocity = gun.transform.rotation*vel;  

                Destroy(p.gameObject, 0.2f);     // Projectile has a lifetime of 0.2 second

                break;
        }
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
