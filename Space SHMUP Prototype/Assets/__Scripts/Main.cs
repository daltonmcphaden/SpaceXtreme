using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT; // Weapon Dictionary

    [Header("Set in Inspector")]
    public GameObject[]         prefabEnemies; // Enemy Prefabs
    public float                enemySpawnPerSecond; // Spawn Rate 
    public float                enemyDefaultPadding = 1.5f;
    public WeaponDefinition[]   weaponDefinitions; // Weapon Definition Array
    public GameObject           prefabPowerUp;      // this will hold the prefab for all powerups
    
    private float _numEnemy = 10, _numLevel = 1; //number of enemys spawned and level number
    private int _enemyClasses = 3; //how many different enemy types there are
    static public float enemysLeft = 10; //how many enemys are left

    public Text currScoreText, highScoreText, restartText, gameOverText, levelText; // all UI text
    private bool _gameOver = false, _restart = false, allSpawned = false;
    public WeaponType[]         powerUpFrequency = new WeaponType [] {WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield, WeaponType.spray, WeaponType.spray};     // Frequency of each powerup

    private BoundsCheck _bndCheck; // Bounds Check Object

    public void ShipDestroyed( Enemy e ) {      // Called by an enemy ship each time one is destroyed
        // Potentially generate a powerup
        if (Random.value <= e.powerUpDropChance) {      // Random value between 0 and 1 generated, each ship has its own drop chance
            // Choose which powerup to pick
            // Pick one from the possibilities in powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length);    // picks a random value in the range
            
            Debug.Log(powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];
            // Spawn a powerup
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            // Set it to the proper weapontype
            pu.SetType(puType);                     // Once a powerup is selected, the powerup SetType method handles the colour and text

            // Set it to the position of the destroyed ship
            pu.transform.position = e.transform.position;
        }
    }

    void Awake()
    {
        S = this;
        _bndCheck = GetComponent<BoundsCheck>();

        StartCoroutine(SpawnEnemy()); // Creates enemy objects from the prefabs

        //dictionary with weapontype as the key
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();

        foreach (WeaponDefinition def in weaponDefinitions) // take each weapon from the array and put it in the dictionary
            WEAP_DICT.Add(def.type, def);

        Score.score = 0; // set score to 0
        SetCurrentScore(); // update text box
        SetHighScore(); // update high score text box
    }

    void SetCurrentScore() // update text box method
    {
        currScoreText.text = "Score: " + Score.score.ToString();
    }

    void SetHighScore()
    {
        highScoreText.text = "High Score: " + Score.highScore.ToString();
    }

    void Update() 
    {
        SetCurrentScore();

        if (_restart) //if restart is true and r has been pressed restart the game
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                DelayedRestart(2f);
            }
        }
    }

    public IEnumerator SpawnEnemy() //spawns enemies
    {
        yield return new WaitForSeconds(2); //2 second before game starts 

        while (true)//loop that makes waves of enemys spawn infinitly to make constant levels until hero ship is destroyed
        {
            levelText.text = "Level " + _numLevel;//text that tells you what level your on

            if (!allSpawned)//makes sure the for loop only runs once per level
            {
                for (int i = 0; i < _numEnemy; i++)
                {
                    // The enemy to be spawned is randomly selected from the array containing each type of enemy
                    int ndx = Random.Range(0, _enemyClasses);
                    GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]); //instantiates enemy prefabs

                    float enemyPadding = enemyDefaultPadding;
                    if (go.GetComponent<BoundsCheck>() != null)
                    {
                        enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
                    }

                    Vector3 pos = Vector3.zero;
                    // min and max x spawn components
                    float xMin = -_bndCheck.camWidth + enemyPadding;
                    float xMax = _bndCheck.camWidth - enemyPadding;

                    // Enemy_1 will spawn from the corners
                    if (ndx == 1)
                    {
                        int p = Random.Range(0, 2); //randomly selects either the left or right
                        if (p == 0)
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
                        pos.x = Random.Range(xMin, xMax);
                    }

                    pos.y = _bndCheck.camHeight + enemyPadding;
                    go.transform.position = pos;

                    if (_gameOver) // if the hero ship dies this will break the spawn loop and desplay restart text
                    {
                        restartText.text = "Press 'R' For Restart";
                        _restart = true;
                        break;
                    }

                    yield return new WaitForSeconds(1f / enemySpawnPerSecond);//wait between spawning enemys
                }

                if (_numLevel== 2)//add another class of enemys for level 3
                {
                    _enemyClasses++;
                }

                allSpawned = true; //all enemys have been spawned
            }

            if (_gameOver) // if the hero ship dies this will break the spawn loop and desplay restart text
            {
                restartText.text = "Press 'R' For Restart";
                _restart = true;
                break;
            }
            
            if (enemysLeft <= 0) //once all the enemys in this level have been destroyed
            {
                _numLevel++; //increment level
                _numEnemy += 10; //make more enemys than previous level
                enemysLeft = _numEnemy; //reset enemys left
                allSpawned = false; //alow another wave to be spawned
                yield return new WaitForSeconds(3); //wait 3 seconds before next wave
            }
            yield return new WaitForSeconds(0.1f);//wait stops infinite loop from crashing game
        }
    }

    public void DelayedRestart(float delay) {
        if(Score.score > Score.highScore){
            Score.highScore = Score.score;
            SetHighScore();
        }
        // Invoke the Restart() method in delay seconds
        Invoke("Restart", delay);
    }

    public void Restart() { // restarts the game
        SceneManager.LoadScene("_Scene_0");
    }

    static public WeaponDefinition GetWeaponDefinition(WeaponType weaponType) // Searches Weapon Dictionary for the Weapon type and returns Weapon type
    {
        if (WEAP_DICT.ContainsKey(weaponType))
        {
           // Debug.Log(WEAP_DICT[weaponType]);
            return WEAP_DICT[weaponType];
        }

        return new WeaponDefinition();
    }

    public void GameOver() // when game is over it will display text
    {
        gameOverText.text = "Game Over";
        _gameOver = true;
    }
}
