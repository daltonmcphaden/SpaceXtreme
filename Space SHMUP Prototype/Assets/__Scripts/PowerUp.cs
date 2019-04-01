using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    // x holds a min value and y a max value for a Random.Range() that will be called later
    public Vector2          rotMinMax = new Vector2(15,90);
    public Vector2          driftMinMax = new Vector2(.25f,2);
    public float            lifeTime = 6f; // Seconds until the PowerUp exists
    public float            fadeTime = 4f; // Seconds it spends fading

    [Header("Set Dynamically")]
    public WeaponType       type;   // the type of the PowerUp
    public GameObject       cube;   // Reference to the Cube child
    public TextMesh         letter; // Reference to the TextMesh
    public Vector3          rotPerSecond;   // Euler rotation speed
    public float            birthTime;

    private Rigidbody       _rigid;
    private BoundsCheck     _bndCheck;
    private Renderer        _cubeRend;

    void Awake() {
        // Find the Cube reference
        cube = transform.Find("Cube").gameObject;
        // Find the TextMesh and other components
        letter = GetComponent<TextMesh>();
        _rigid = GetComponent<Rigidbody>();
        _bndCheck = GetComponent<BoundsCheck>();
        _cubeRend = cube.GetComponent<Renderer>();

        // Set a random velocity
        Vector3 vel = Random.onUnitSphere; // Gets random XYZ velocity
        vel.z = 0; // Flattens the velocity to the XY plane
        vel.Normalize();    // Gives vel a vector length of 1

        // Set the velocity length to something between the x and y values of driftMinMax
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);      
        _rigid.velocity = vel;      // assigns the velocity to the rigidbody

        // Set the rotation of this GambeObject to R:[0,0,0]
        transform.rotation = Quaternion.identity;       // gives it no rotation

        // Set up the rotPerSecond for the Cube child using rotMinMax x and y
        rotPerSecond = new Vector3(
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y)  );
        
        birthTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond*Time.time); // Rotate the cube every second

        // Fade out the PowerUp over time
        // Gven the default values, a PowerUp will exist for 6 seconds and fade out over 4 seconds
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        // For lifetime seconds, u will be <= 0, then it will transitition to 1 over the course of fadeTime seconds

        // If u >= 1, destroy this PowerUp
        if (u >= 1) {
            Destroy(this.gameObject);
            return;
        }

        // Use u to determine the alpha value of the Cube & Letter
        if(u > 0) {
            Color c = _cubeRend.material.color;
            c.a = 1f - u;
            _cubeRend.material.color = c;
            // Fade the letter too, just not as much
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }

        if (!_bndCheck.isOnScreen) {
            // if the powerup has drifted entirely off screen, destroy it
            Destroy(gameObject);
        }
    }

    public void SetType( WeaponType wt ){
        // Grab the WeaponDefinition from Main
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        // Set the color of the Cube child
        _cubeRend.material.color = def.color;
        letter.color = def.color;
        letter.text = def.letter;
        type = wt;                  // set the type
    }

    public void AbsorbedBy( GameObject target ) {
        // This function is called by the Hero class when a PowerUp is collected
        Destroy(this.gameObject);
    }

}
