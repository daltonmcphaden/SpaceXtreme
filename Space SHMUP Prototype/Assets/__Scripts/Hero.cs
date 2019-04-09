using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    static public Hero S;

    public float speed = 30;                    // Ship speed
    public float rollMult = -45;                // Amount that the ship will roll on movement
    public float pitchMult = 30;                // Amount that the ship with rotate on movement
    public float gameRestartDelay = 2f;         // Timed delay for restart
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;          // Default speed
    public Weapon [] weapons;                   // Array for holding weapons

    public Text [] weaponHUD;                   // Array containing Text of each weapon for the HUD in the top right corner
    private WeaponType _currentWeaponType;      // Keeps track of the current weapon type in use
    public int [] weaponLevels = { 1, 1, 1, 1 };        // Array that holds the current Rank/Level of each weapon

    [Header("Set Dynamically")]
    [SerializeField]
    private float    _shieldLevel = 4;          // Default shield level is 4

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

        // Set each of the text items in the WeaponHUD array
        SetBlasterText();
        SetSpreadText();
        SetSprayText();
        SetMissileText();

        ActivateText(weaponHUD[0]);      // Set blasterText to be bold and italic since it is the starting weapon
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
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null){        // Press spacebar to fire
            fireDelegate();
        }

        if (Input.GetKeyDown(KeyCode.G)){       // Press G to switch weapons
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
            shieldLevel--;          // Decrease shield level
            Main.enemyList.Remove(gameObject);  //remove from enemy list
            Main.enemysLeft--;                  // Decrease the number of enemies left
            Destroy(go);                        // Destroy the enemy object
        } 

        else if (go.tag == "PowerUp") {
            // If the shield was triggered by a PowerUp
            AbsorbPowerUp(go);
        } 

        else if (go.tag == "ProjectileEnemy") {
            shieldLevel--;                      // decrease shield level
            Destroy(go);
        }
        
        else {
            print("Triggered by non-enemy: " + go.name);    // Exception handler
        }
    }

    public void AbsorbPowerUp( GameObject go ) {

        aSource.PlayOneShot(puGet);             // Play powerup sound
        PowerUp pu = go.GetComponent<PowerUp>();
        int level = 1;
        switch (pu.type) {
            
            case WeaponType.shield:     // if powerup is a shield, increase the shield level by 1
                shieldLevel++;
                break;

            case WeaponType.blaster:
                if (weaponLevels[0] < 3) {      // If the level is less than 3, increase its level
                    ++weaponLevels[0];
                    SetBlasterText();           // Update the HUD text
                    if (weaponLevels[0] == 2)
                        Weapon._vol = 0.267f;
                    if (weaponLevels[0] == 3)
                        Weapon._vol = 0.16f;
                }
                level = weaponLevels[0];
                break;
                
            case WeaponType.spread:
                if (weaponLevels[1] < 3){       // If the level is less than 3, increase its level
                    ++weaponLevels[1];
                    SetSpreadText();            // Update the HUD text
                }
                level = weaponLevels[1];
                break;

            case WeaponType.spray:
                if (weaponLevels[2] < 3){       // If the level is less than 3, increase its level
                    ++weaponLevels[2];
                    SetSprayText();             // Update the HUD text
                }
                level = weaponLevels[2];
                break;
            
            case WeaponType.missile:
                if (weaponLevels[3] < 3){       // If the level is less than 3, increase its level
                    ++weaponLevels[3];
                    SetMissileText();           // Update the HUD text
                }
                level = weaponLevels[3];
                break;

        }

        if (pu.type == _currentWeaponType) {            // If the current weapon being used is the same as the powerup collected, show the change in weapon right away
            SetActiveWeapon(_currentWeaponType, level);     // Set the active weapon with its according level
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
        foreach (Weapon w in weapons) {         // Clear all weapon slots
            w.SetType(WeaponType.none);
        }
    }

    void SetActiveWeapon(WeaponType wt, int level){

        ClearWeapons();     // Start by clearing all weapons

        switch (wt) {       // Switch statement with the passed weapon type
            
            case WeaponType.blaster:

                switch(level) {
                    case 3:     // Rank 3 blaster: 5 blasters
                        weapons[4].SetType(WeaponType.blaster);
                        weapons[3].SetType(WeaponType.blaster);
                        goto case 2;
                    case 2:     // Rank 2 blaster: 3 blasters
                        weapons[2].SetType(WeaponType.blaster);
                        weapons[1].SetType(WeaponType.blaster);
                        goto case 1;
                    case 1:     // Rank 1 blaster: 1 blaster
                        weapons[0].SetType(WeaponType.blaster);
                        break;
                }
                break;

            case WeaponType.spread:
                // Spread is always just a single gun
                weapons[0].SetType(WeaponType.spread);
                
                break;

            case WeaponType.spray:
                switch(level) {
                    case 1:     // Rank 1 spray, slow fire rate
                        Main.S.weaponDefinitions[3].delayBetweenShots = 1f;
                        break;
                    case 2:     // Rank 2 spray, medium fire rate
                        Main.S.weaponDefinitions[3].delayBetweenShots = 0.6f;
                        break;
                    case 3:     // Rank 3 spray, high fire rate
                        Main.S.weaponDefinitions[3].delayBetweenShots = 0.4f;
                        break;
                }
                
                weapons[0].SetType(WeaponType.spray);       // Front weapon is set

                for (int i = 5; i < weapons.Length; i++){       // The remaining 7 are set in a for loop
                    weapons[i].SetType(WeaponType.spray);
                }
                
                
                break;
            
            case WeaponType.missile:

                switch(level) {
                    case 1:     // Rank 1 missile, slow fire rate
                        Main.S.weaponDefinitions[4].delayBetweenShots = 1f;
                        break;
                    case 2:     // Rank 2 missile, medium fire rate
                        Main.S.weaponDefinitions[4].delayBetweenShots = 0.8f;
                        break;
                    case 3:     // Rank 3 missile, high fire rate
                        Main.S.weaponDefinitions[4].delayBetweenShots = 0.6f;
                        break;
                }

                weapons[12].SetType(WeaponType.missile);        // Missile is always weapon slot 12

                break;
        }
    }

    void CycleCurrentWeapon(){
        int levelOfNextWeapon = 1;      // Stores the level of the weapon that is being switch to (ie the next weapon)
        switch(_currentWeaponType){

            case WeaponType.blaster:

                levelOfNextWeapon = weaponLevels[1];        // Get level of next weapon on the list
                _currentWeaponType = WeaponType.spread;     // Get type of next weapon on the list
                ActivateText(weaponHUD[1]);                 // Set the text of this weapon
                break;
            
            case WeaponType.spread:

                levelOfNextWeapon = weaponLevels[2];        // Get level of next weapon on the list
                _currentWeaponType = WeaponType.spray;      // Get type of next weapon on the list
                ActivateText(weaponHUD[2]);                 // Set the text of this weapon
                break;

            case WeaponType.spray:

                levelOfNextWeapon = weaponLevels[3];        // Get level of next weapon on the list
                _currentWeaponType = WeaponType.missile;    // Get type of next weapon on the list
                ActivateText(weaponHUD[3]);                 // Set the text of this weapon
                break;
            
            case WeaponType.missile:

                levelOfNextWeapon = weaponLevels[0];        // Get level of next weapon on the list
                _currentWeaponType = WeaponType.blaster;    // Get type of next weapon on the list
                ActivateText(weaponHUD[0]);                 // Set the text of this weapon
                break;
        }

        SetActiveWeapon(_currentWeaponType, levelOfNextWeapon); // Activate the weapon based on the level and type gained above
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

    // Set the font to be italic and bold to better show which weapon is active
    void ActivateText(Text item) {
        item.fontStyle = FontStyle.BoldAndItalic; 
        foreach (Text t in weaponHUD){
            if (t != item){
                t.fontStyle = FontStyle.Normal; 
            }
        }
    }

    // Set all weapon levels back to 1 and have the HUD elements reflect it
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
