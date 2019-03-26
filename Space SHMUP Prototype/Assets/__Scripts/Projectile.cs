using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile : MonoBehaviour
{
    private BoundsCheck _bndCheck;
    private Renderer _rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;

    // property to set or get the weapon type
    public WeaponType type
    {
        get
        {
            return (_type);
        }
        set
        {
            SetType(value);
        } 
    }


    void Awake() // gets components
    {
        _bndCheck = GetComponent<BoundsCheck>();
        _rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (_bndCheck.offUp)
            Destroy(gameObject);
    }

    //sets the _type feild and colors the projectile to match the weaponDefinition
    public void SetType(WeaponType eType)
    {
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        _rend.material.color = def.projectileColor;
    }
}
