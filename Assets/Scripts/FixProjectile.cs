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
        
        _rb.AddForce(-transform.position.normalized*30, ForceMode.Acceleration);
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {   
            
            _rb.velocity = Vector3.Cross(Vector3.Cross(collision.gameObject.transform.forward,PlayerController._lastForward)*100,collision.gameObject.transform.forward)*-1 + collision.gameObject.GetComponentInParent<Rigidbody>().velocity;
            
            print("COLLISION" + Vector3.Cross(collision.gameObject.transform.forward,PlayerController._lastForward));
        }
       
    }
}
