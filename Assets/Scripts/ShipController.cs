using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{   
    private Rigidbody _rb;
    private GravityField _gF;
    public Vector2 zxRotationValue;
    public float rightYRotationValue;
    public float leftYRotationValue;
    public float mainEngineValue;
    public float breakEngineValue;
    public float takeOffEngineValue;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
    }

    void FixedUpdate(){
        _rb.AddForce(-transform.forward*mainEngineValue*10000);
        _rb.AddForce(transform.forward*breakEngineValue*10000);
        _rb.AddForce(transform.up*takeOffEngineValue*10000);
        _rb.AddTorque(transform.right*Math.Clamp(zxRotationValue.y,-1.0f,0f)*-1*10000);
        _rb.AddTorque(-transform.right*Math.Clamp(zxRotationValue.y,0.0f,1.0f)*10000);
        _rb.AddTorque(transform.forward*Math.Clamp(zxRotationValue.x,0.0f,1.0f)*10000);
        _rb.AddTorque(-transform.forward*Math.Clamp(zxRotationValue.x,-1.0f,0f)*-1*10000);
        _rb.AddTorque(-transform.up*leftYRotationValue*10000);
        _rb.AddTorque(transform.up*rightYRotationValue*10000);

        print(_gF.GetTotalFieldForceForBody(transform.position));
        _rb.AddForce(_gF.GetTotalFieldForceForBody(transform.position),ForceMode.Acceleration);
    }

    public void SetupGravityField(GravityField gravity){
        _gF = gravity;
    }
}
