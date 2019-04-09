using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Set In Inspector")]
    public float    rotationsPerSecond = 0.1f;

    [Header("Set Dynamically")]
    public int      levelShown = 4;     // Starting shield level is 4

    // This private variable will not appear in the Inspector
    private Material _mat;

    // Start is called before the first frame update
    void Start()
    {
        _mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        // Read the current shield level from the Hero Singleton
        int currLevel = Mathf.FloorToInt( Hero.S.shieldLevel );
        if (levelShown != currLevel){
            levelShown = currLevel;
            // Adjust the texture offset to show different shield level
            _mat.mainTextureOffset = new Vector2( 0.2f*levelShown, 0 );
        }
        // Rotate the shield a bit every frame in a time-based way
        float rZ = -(rotationsPerSecond*Time.time*360) % 360f;
        transform.rotation = Quaternion.Euler(0,0,rZ);
    }
}
