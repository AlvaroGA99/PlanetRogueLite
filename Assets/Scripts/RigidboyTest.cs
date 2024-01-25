using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class RigidboyTest : MonoBehaviour
{
    Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.jKey.wasReleasedThisFrame){
            _rb.velocity = new Vector3();
        }
    }

    void FixedUpdate(){
       
        

        if (Keyboard.current.kKey.isPressed){
             _rb.AddForce(25*transform.forward);
        }
    }
}
