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
    public bool breakActivated;

    private Vector2 overridezxRotationValue;
    private float overriderightYRotationValue;
    private float overrideleftYRotationValue;
    private float overridemainEngineValue;
    private float overridebreakEngineValue;
    private float overridetakeOffEngineValue;
    private float overridelandingEngineValue;
    private bool overridebreakActivated;
    public Energy energyObject;
    //public float _energy;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
        if (energyObject.energy > 0)
        {   
            overridezxRotationValue = zxRotationValue;
            overriderightYRotationValue = rightYRotationValue;
            overrideleftYRotationValue = leftYRotationValue;
            overridemainEngineValue = mainEngineValue;
            overridebreakEngineValue = breakEngineValue;
            overridetakeOffEngineValue = takeOffEngineValue;
            overridelandingEngineValue = landingEngineValue;
            overridebreakActivated = breakActivated;

            Vector3 localAngularVelocity = transform.worldToLocalMatrix.MultiplyVector(_rb.angularVelocity);
            print(localAngularVelocity);
            if (breakActivated)
            {
                bool stopFlag = false;
                
                localAngularVelocity = new Vector3(-localAngularVelocity.x,localAngularVelocity.z,localAngularVelocity.y);
                
                if (localAngularVelocity.x > 0.05)
                {
                    overridezxRotationValue.y = -1;
                }
                else if (localAngularVelocity.x < -0.05)
                {
                    overridezxRotationValue.y = 1;
                }
                // else if( localAngularVelocity.x != 0){
                //     overridezxRotationValue.y = 0;
                //     localAngularVelocity.x = 0;
                //     stopFlag = true;
                // }   
                
                if (localAngularVelocity.z > 0.05)
                {
                    overrideleftYRotationValue = 1;
                }
                else if (localAngularVelocity.z < -0.05)
                {
                    overriderightYRotationValue = 1;
                }
                // else if( localAngularVelocity.z != 0){
                //     overrideleftYRotationValue = 0;
                //     overriderightYRotationValue = 0;
                //     localAngularVelocity.z = 0;
                //     stopFlag = true;
                // } 

                if (localAngularVelocity.y > 0.05)
                {
                    overridezxRotationValue.x = -1;
                }
                else if (localAngularVelocity.y < -0.05)
                {
                    overridezxRotationValue.x = 1;
                }
                //  else if( localAngularVelocity.y != 0){
                //     overridezxRotationValue.x = 0;
                //     localAngularVelocity.y = 0;
                //     stopFlag = true;
                // } 
        
                // if(stopFlag){
                //     _rb.angularVelocity = transform.localToWorldMatrix.MultiplyVector(localAngularVelocity);
                // }
            }
            
            
            _rb.AddForce(-transform.forward * overridemainEngineValue * 10000);
            _rb.AddForce(transform.forward * overridebreakEngineValue * 10000);
            _rb.AddForce(transform.up * overridetakeOffEngineValue * 10000);
            _rb.AddForce(-transform.up * overridelandingEngineValue * 10000);
            _rb.AddTorque(transform.right * Math.Clamp(overridezxRotationValue.y, -1.0f, 0f) * -1 * 5000);
            _rb.AddTorque(-transform.right * Math.Clamp(overridezxRotationValue.y, 0.0f, 1.0f) * 5000);
            _rb.AddTorque(transform.forward * Math.Clamp(overridezxRotationValue.x, 0.0f, 1.0f) * 5000);
            _rb.AddTorque(-transform.forward * Math.Clamp(overridezxRotationValue.x, -1.0f, 0f) * -1 * 5000);
            _rb.AddTorque(-transform.up * overrideleftYRotationValue * 5000);
            _rb.AddTorque(transform.up * overriderightYRotationValue * 5000);
        }



        //print(_gF.GetTotalFieldForceForBody(transform.position));
        _rb.AddForce(_gF.GetTotalFieldForceForBody(transform.position), ForceMode.Acceleration);
    } 

    void Update()
    {
        // if(_energy > 0){
        //     _energy -= Time.deltaTime*(mainEngineValue+breakEngineValue+takeOffEngineValue+landingEngineValue+zxRotationValue.y+leftYRotationValue+rightYRotationValue)/7;
        // }
        // if(_energy< 0){
        //     _energy = 0;
        // }
        energyObject.UpdateEnergy(Time.deltaTime * (overridemainEngineValue + overridebreakEngineValue + overridetakeOffEngineValue + overridelandingEngineValue + Math.Abs(overridezxRotationValue.y) + Math.Abs(overridezxRotationValue.x) + overrideleftYRotationValue + overriderightYRotationValue) / 8);
    }

    public void SetupGravityField(GravityField gravity)
    {
        _gF = gravity;
    }

    // private void UpdateEnergy(){
    //     _energyImage.transform.localScale = new Vector3(_energy/10,1,1);
    // }
}
