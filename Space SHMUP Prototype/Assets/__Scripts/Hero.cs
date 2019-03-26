using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S;

    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;

    public Weapon weapon;

    [Header("Set Dynamically")]
    [SerializeField]
    private float    _shieldLevel = 4;

    private GameObject _lastTriggerGo = null;

    //declare new delagate
    public delegate void WeaponFireDelegate();
    //create weaponfiredeligate field
    public WeaponFireDelegate fireDelegate;

    private void Awake()
    {
        if (S == null)      // Singleton design pattern, ensures there is only one hero
        {
            S = this;
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Move Hero
        float xAxis = Input.GetAxis("Horizontal"); 
        float yAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Hero ship also rotates when it moves
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, -xAxis * pitchMult, 0);

        // allow the ship to fire using fire delegate
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null){
            fireDelegate();
        }
        // Allows swapping between the blaster and the spread weapon 
        if (Input.GetKeyDown(KeyCode.H)) {
          if (weapon.type == WeaponType.blaster){
              weapon.type = WeaponType.spread;
          }
          else {
              weapon.type = WeaponType.blaster;
          }
        }
        
    }

    void OnTriggerEnter(Collider other){
        // Collision detection for the shield
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        // Make sure it's not the same triggering gameObject as last time
        if (go == _lastTriggerGo)
            return;

        _lastTriggerGo = go;

        if (go.tag == "Enemy"){     // If the shield was triggered by an enemy
            shieldLevel--;
            Destroy(go);
        } else {
            print("Triggered by non-enemy " + go.name);
        }
    }

// property for shield level
    public float shieldLevel {
        get {
            return (_shieldLevel);
        }
        set {
            _shieldLevel = Mathf.Min(value, 4);
            // If the shield is going to be set to less than zero
            if (value < 0) {
                Destroy(this.gameObject);
                // Tell Main.S to restart the game after a delay
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }
}
