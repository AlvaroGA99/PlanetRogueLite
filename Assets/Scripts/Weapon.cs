using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{

    public static event Action<float> OnReleaseShot;
    float force;
    private float _energyPoints;
    [SerializeField]
    private GameObject child;

    public Image Energy;

    // Start is called before the first frame update
    void Start()
    {
        _energyPoints = 1.0f;
        // targetBody = null;s
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {  
            if(_energyPoints > 0){
                _energyPoints -= Time.deltaTime/2;
                UpdateEnergy();
                if (force < 2 )
                {
                    child.transform.localScale -= Vector3.one*1.125f*Time.deltaTime;
                    force += Time.deltaTime;
                }
            }
            
        }
        // else{
        //     if(_energyPoints < 1.0f){
        //         _energyPoints += Time.deltaTime/2;
        //         UpdateEnergy();
        //     }
        // }
        if (Mouse.current.leftButton.wasReleasedThisFrame){
            OnReleaseShot?.Invoke(force/2);
            child.transform.localScale = Vector3.one*3;
            _energyPoints =  1.0f;
            UpdateEnergy();
            force = 0;
        }
    }

    private void UpdateEnergy(){
        Energy.transform.localScale = new Vector3(_energyPoints,1,1);
    }
    // void OnTriggerEnter(Collider other){
    //     if(other.tag == "Projectile"){
    //        colliding = true;
    //     }   
    // }

}

