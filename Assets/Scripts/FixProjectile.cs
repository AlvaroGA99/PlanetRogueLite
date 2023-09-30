using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody _rb;
    private Vector3 _savedVector;
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
        
        _rb.AddForce(-transform.position.normalized*50, ForceMode.Acceleration);
    }
    
    // private void OnTriggerStay(Collider collision)
    // {
    //     if (collision.gameObject.tag == "Weapon")
    //     {   
    //         transform.position = collision.ClosestPointOnBounds(transform.position);
    //         //_rb.velocity = vel;
            
    //         print("COLLISION" + Vector3.Cross(collision.gameObject.transform.forward,PlayerController._lastForward));
    //     }
       
    // }

     private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {  Vector3 vel = collision.transform.right +  Vector3.Cross(Vector3.Cross(collision.gameObject.transform.forward,PlayerController._lastForward)*100,collision.transform.forward)*-1 + collision.gameObject.GetComponentInParent<Rigidbody>().velocity + collision.transform.up;
            _savedVector = transform.position - collision.transform.position;
            transform.position += vel.normalized*0.05f;
            _rb.velocity = vel;
            
            print("COLLISION" + Vector3.Cross(collision.gameObject.transform.forward,PlayerController._lastForward));
        }
       
    }
}
