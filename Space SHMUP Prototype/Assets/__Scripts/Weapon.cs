using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    none, blaster, spread, phaser, missile, laser, sheild
}

[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; //letter to show on the powerup
    public Color color = Color.white; //color of collar & power up
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0, continousDamage = 0, delayBetweenShots = 0, velocity = 20;
}
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime;
    private Renderer _collarRend;

    // Start is called before the first frame update
    void Start()
    {
        collar = transform.Find("Collar").gameObject;
        _collarRend = collar.GetComponent<Renderer>();

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

    public WeaponType type
    {
        get { return _type; }
        set { SetType(value); }
    }

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
        _collarRend.material.color = def.color;
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

        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;

            case WeaponType.spread:
             
                //right projectile
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(30, Vector3.back); 
                p.rigid.velocity = p.transform.rotation * vel;
                //center projectile
                p = MakeProjectile();
                p.rigid.velocity = vel;
                //left projectile
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-30, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;
        }
    }
    public Projectile MakeProjectile()
    {
        GameObject gameObj = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Hero")
        {
            gameObj.tag = "ProjectileHero";
            gameObj.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            gameObj.tag = "ProjectileEnemy";
            gameObj.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        gameObj.transform.position = collar.transform.position;
        gameObj.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = gameObj.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;
        return p;
    }
}
