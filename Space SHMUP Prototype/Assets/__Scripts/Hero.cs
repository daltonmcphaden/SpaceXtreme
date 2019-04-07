using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    static public Hero S;

    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon [] weapons;

    public Text [] weaponHUD;
    private WeaponType _currentWeaponType;      // Keeps track of the current weapon type in use
    public int [] weaponLevels = { 1, 1, 1, 1 };

    [Header("Set Dynamically")]
    [SerializeField]
    private float    _shieldLevel = 4;

    private GameObject _lastTriggerGo = null;

    //declare new delagate
    public delegate void WeaponFireDelegate();
    //create weaponfiredeligate field
    public WeaponFireDelegate fireDelegate;
    public AudioClip puGet, swap;
    public AudioSource aSource;

    private void Awake()
    {
        if (S == null)      // Singleton design pattern, ensures there is only one hero
        {
            S = this;
            _currentWeaponType = WeaponType.blaster;        // Starting weapon type is always the blaster
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }

        SetBlasterText();
        SetSpreadText();
        SetSprayText();
        SetMissileText();

        ActivateText(weaponHUD[0]);      // Set blasterText to be bold since it is the starting weapon
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

        if (Input.GetKeyDown(KeyCode.G)){
            CycleCurrentWeapon();
            aSource.PlayOneShot(swap,0.5f);
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
            Main.enemyList.Remove(gameObject);//remove from enemy list
            Main._enemysLeft--;
            Destroy(go);
        } 

        else if (go.tag == "PowerUp") {
            // If the shield was triggered by a PowerUp
            AbsorbPowerUp(go);
        } 

        else if (go.tag == "ProjectileEnemy") {
            shieldLevel--;
            Destroy(go);
        }
        
        else {
            print("Triggered by non-enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp( GameObject go ) {

        aSource.PlayOneShot(puGet);
        PowerUp pu = go.GetComponent<PowerUp>();
        int level = 1;
        switch (pu.type) {
            
            case WeaponType.shield:     // if powerup is a shield, increase the shield level by 1
                shieldLevel++;
                break;

            case WeaponType.blaster:
                if (weaponLevels[0] < 3) {
                    ++weaponLevels[0];
                    SetBlasterText();
                    if (weaponLevels[0] == 2)
                        Weapon._vol = 0.267f;
                    if (weaponLevels[0] == 3)
                        Weapon._vol = 0.16f;
                }
                level = weaponLevels[0];
                break;
                
            case WeaponType.spread:
                if (weaponLevels[1] < 3){
                    ++weaponLevels[1];
                    SetSpreadText();
                }
                level = weaponLevels[1];
                break;

            case WeaponType.spray:
                if (weaponLevels[2] < 3){
                    ++weaponLevels[2];
                    SetSprayText();
                }
                level = weaponLevels[2];
                break;
            
            case WeaponType.missile:
                if (weaponLevels[3] < 3){
                    ++weaponLevels[3];
                    SetMissileText();
                }
                level = weaponLevels[3];
                break;

        }

        if (pu.type == _currentWeaponType) {            // If the current weapon being used is the same as the powerup collected, show the change in weapon right away
            SetActiveWeapon(_currentWeaponType, level);
        }

        pu.AbsorbedBy( this.gameObject );
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
                // Tell Main.S to restart the game
                Main.S.GameOver();
            }
        }
    }

    void ClearWeapons() {
        foreach (Weapon w in weapons) {
            w.SetType(WeaponType.none);
        }
    }

    void SetActiveWeapon(WeaponType wt, int level){

        ClearWeapons();

        switch (wt) {
            
            case WeaponType.blaster:

                switch(level) {
                    case 3:
                        weapons[4].SetType(WeaponType.blaster);
                        weapons[3].SetType(WeaponType.blaster);
                        goto case 2;
                    case 2:
                        weapons[2].SetType(WeaponType.blaster);
                        weapons[1].SetType(WeaponType.blaster);
                        goto case 1;
                    case 1:
                        weapons[0].SetType(WeaponType.blaster);
                        break;
                }
                break;

            case WeaponType.spread:

                weapons[0].SetType(WeaponType.spread);
                
                break;

            case WeaponType.spray:
                switch(level) {
                    case 1:
                        Main.S.weaponDefinitions[3].delayBetweenShots = 1f;
                        break;
                    case 2:
                        Main.S.weaponDefinitions[3].delayBetweenShots = 0.6f;
                        break;
                    case 3:
                        Main.S.weaponDefinitions[3].delayBetweenShots = 0.4f;
                        break;
                }
                
                weapons[0].SetType(WeaponType.spray);

                for (int i = 5; i < weapons.Length; i++){
                    weapons[i].SetType(WeaponType.spray);
                }
                
                
                break;
            
            case WeaponType.missile:

                switch(level) {
                    case 1:
                        Main.S.weaponDefinitions[4].delayBetweenShots = 1f;
                        break;
                    case 2:
                        Main.S.weaponDefinitions[4].delayBetweenShots = 0.8f;
                        break;
                    case 3:
                        Main.S.weaponDefinitions[4].delayBetweenShots = 0.6f;
                        break;
                }

                weapons[12].SetType(WeaponType.missile);

                break;
        }
    }

    void CycleCurrentWeapon(){
        int levelOfNextWeapon = 1;      // Stores the level of the weapon that is being switch to (ie the next weapon)
        switch(_currentWeaponType){

            case WeaponType.blaster:

                levelOfNextWeapon = weaponLevels[1];
                _currentWeaponType = WeaponType.spread;
                ActivateText(weaponHUD[1]);
                break;
            
            case WeaponType.spread:

                levelOfNextWeapon = weaponLevels[2];
                _currentWeaponType = WeaponType.spray;
                ActivateText(weaponHUD[2]);
                break;

            case WeaponType.spray:

                levelOfNextWeapon = weaponLevels[3];
                _currentWeaponType = WeaponType.missile;
                ActivateText(weaponHUD[3]);
                break;
            
            case WeaponType.missile:

                levelOfNextWeapon = weaponLevels[0];
                _currentWeaponType = WeaponType.blaster;
                ActivateText(weaponHUD[0]);
                break;
        }

        SetActiveWeapon(_currentWeaponType, levelOfNextWeapon);
    }
    
    void SetBlasterText(){
        weaponHUD[0].text = "Blaster " + new string('I', weaponLevels[0]);
    }

    void SetSpreadText() {
        weaponHUD[1].text = "Spread " + new string('I', weaponLevels[1]);
    }

    void SetSprayText() {
        weaponHUD[2].text = "Spray " + new string('I', weaponLevels[2]);
    }

    void SetMissileText() {
        weaponHUD[3].text = "Missile " + new string('I', weaponLevels[3]);
    }

    void ActivateText(Text item) {
        item.fontStyle = FontStyle.BoldAndItalic;
        foreach (Text t in weaponHUD){
            if (t != item){
                t.fontStyle = FontStyle.Normal; 
            }
        }
    }

    public void ResetWeaponLevels(){
        for (int i = 0; i < weaponLevels.Length; i++ ){
            weaponLevels[i] = 1;
        }
        SetBlasterText();
        SetSpreadText();
        SetSprayText();
        SetMissileText();

        Weapon._vol = 0.8f;
        SetActiveWeapon(_currentWeaponType, 1);
    }

}
