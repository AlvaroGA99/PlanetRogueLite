using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{

    public static event Action<float> OnReleaseShot;
    float force;

    // Start is called before the first frame update
    void Start()
    {

        // targetBody = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            if (force < 2)
            {
                force += Time.deltaTime;
            }

            
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame){
            OnReleaseShot?.Invoke(force);
            force = 0;
        }
    }


    // void OnTriggerEnter(Collider other){
    //     if(other.tag == "Projectile"){
    //        colliding = true;
    //     }   
    // }

}

