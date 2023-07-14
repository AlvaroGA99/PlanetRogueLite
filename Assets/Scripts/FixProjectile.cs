using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {   
        
        _rb.AddForce(-transform.position.normalized*10, ForceMode.Acceleration);
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {
            _rb.velocity = (transform.position - collision.transform.position).normalized * 1000;
            
            
            print("COLLISION");
        }
       
    }
}
