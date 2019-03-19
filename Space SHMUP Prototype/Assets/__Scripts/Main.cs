using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;
    public WeaponDefinition[] weaponDefinitions;
    
    private BoundsCheck bndCheck;

    void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);

        //dictionary with weapontype as the key
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();

        foreach (WeaponDefinition def in weaponDefinitions)
            WEAP_DICT.Add(def.type, def);
        
    }

    public void SpawnEnemy()
    {
        // The enemy to be spawned is randomly selected from the array containing each type of enemy
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }
        
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;    
        
        // Enemy_1 will spawn from the corners
        if (ndx == 1)
        {
           int p = Random.Range(0,2);
           if (p==0)
           {
               pos.x = xMin;
           }
           else
           {
               pos.x = xMax;
           }
        }
        // Other enemies spawn anywhere across the top of the screen
        else
        {
            pos.x = Random.Range(xMin,xMax);
        }
        
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);
    }

    public void DelayedRestart(float delay) {
        // Invoke the Restart() method in delay seconds
        Invoke("Restart", delay);
    }

    public void Restart() {
        SceneManager.LoadScene("_Scene_0");
    }

    static public WeaponDefinition GetWeaponDefinition(WeaponType weaponType)
    {
        if (WEAP_DICT.ContainsKey(weaponType))
        {
            return WEAP_DICT[weaponType];
        }

        return new WeaponDefinition();
    }
}
