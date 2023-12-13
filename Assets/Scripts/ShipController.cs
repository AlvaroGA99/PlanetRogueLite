using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public float landingEngineValue;

    
    public Image _energyImage;
    public float _energy;

    // Start is called before the first frame update
    void Start()
    {   
        _energy = 10;
        _rb = GetComponent<Rigidbody>();
        
    }

    void FixedUpdate(){
        if(_energy > 0){
        _rb.AddForce(-transform.forward*mainEngineValue*10000);
        _rb.AddForce(transform.forward*breakEngineValue*10000);
        _rb.AddForce(transform.up*takeOffEngineValue*10000);
        _rb.AddForce(-transform.up*landingEngineValue*10000);
        _rb.AddTorque(transform.right*Math.Clamp(zxRotationValue.y,-1.0f,0f)*-1*10000);
        _rb.AddTorque(-transform.right*Math.Clamp(zxRotationValue.y,0.0f,1.0f)*10000);
        _rb.AddTorque(transform.forward*Math.Clamp(zxRotationValue.x,0.0f,1.0f)*10000);
        _rb.AddTorque(-transform.forward*Math.Clamp(zxRotationValue.x,-1.0f,0f)*-1*10000);
        _rb.AddTorque(-transform.up*leftYRotationValue*10000);
        _rb.AddTorque(transform.up*rightYRotationValue*10000);
        }
        

        print(_gF.GetTotalFieldForceForBody(transform.position));
        _rb.AddForce(_gF.GetTotalFieldForceForBody(transform.position),ForceMode.Acceleration);
    }

    void Update(){
        if(_energy > 0){
            _energy -= Time.deltaTime*(mainEngineValue+breakEngineValue+takeOffEngineValue+landingEngineValue+zxRotationValue.y+leftYRotationValue+rightYRotationValue)/7;
        }
        if(_energy< 0){
            _energy = 0;
        }
        UpdateEnergy();
    }

    public void SetupGravityField(GravityField gravity){
        _gF = gravity;
    }

    private void UpdateEnergy(){
        _energyImage.transform.localScale = new Vector3(_energy/10,1,1);
    }
}
