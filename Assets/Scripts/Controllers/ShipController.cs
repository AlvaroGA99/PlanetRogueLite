using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
    private Rigidbody _rb;
    private GravityField _gF;

    [SerializeField]private TrailRenderer _tr1;
    [SerializeField]private TrailRenderer _tr2;
    [SerializeField]private MeshRenderer _mtr1;
    [SerializeField]private MeshRenderer _mtr2;
    [SerializeField] private Transform childShip;
    public Vector2 zxRotationValue;
    public float rightYRotationValue;
    public float leftYRotationValue;
    public float mainEngineValue;
    public float breakEngineValue;
    public float takeOffEngineValue;
    public float landingEngineValue;
    public bool breakActivated;
    public bool speedAligner;
    public float speedAlignerValue;
    public float speedAlignerValue2;

    private Vector2 overridezxRotationValue;
    private float overriderightYRotationValue;
    private float overrideleftYRotationValue;
    private float overridemainEngineValue;
    private float overridebreakEngineValue;
    private float overridetakeOffEngineValue;
    private float overridelandingEngineValue;
    public Energy energyObject;
    private float speed;

    public Vector3 lastVelocity { get; private set; }

    //[SerializeField] private Collider enterAreaCollider;
    //public float _energy;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        speed = 2;
        //enterAreaCollider.enabled = true;
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

            Vector3 localAngularVelocity = transform.worldToLocalMatrix.MultiplyVector(_rb.angularVelocity);
            Vector3 localVelocity = transform.worldToLocalMatrix.MultiplyVector(_rb.velocity);
            
            _rb.MovePosition(_rb.position - transform.forward*speed);
            _rb.drag = 10*speed;
            _rb.angularDrag = 10;
            // if (breakActivated)
            // {   
            //     _rb.angularDrag = 0;
                

            //     bool xflag = false;
            //     bool yflag = false;
            //     bool zflag = false;
                
            //     localAngularVelocity = new Vector3(-localAngularVelocity.x,localAngularVelocity.z,localAngularVelocity.y);
                
            //     if (localAngularVelocity.x > 0.05)
            //     {
            //         overridezxRotationValue.y = -1;
            //     }
            //     else if (localAngularVelocity.x < -0.05)
            //     {
            //         overridezxRotationValue.y = 1;
            //     }
            //     else if( localAngularVelocity.x != 0){
            //         xflag = true;
            //     }   
                
            //     if (localAngularVelocity.z > 0.05)
            //     {
            //         overrideleftYRotationValue = 1;
            //     }
            //     else if (localAngularVelocity.z < -0.05)
            //     {
            //         overriderightYRotationValue = 1;
            //     }
            //     else if( localAngularVelocity.z != 0){
            //         yflag = true;
            //     } 

            //     if (localAngularVelocity.y > 0.05)
            //     {
            //         overridezxRotationValue.x = -1;
            //     }
            //     else if (localAngularVelocity.y < -0.05)
            //     {
            //         overridezxRotationValue.x = 1;
            //     }
            //     else if( localAngularVelocity.y != 0){
            //          zflag = true;
            //     }  

            //     if(xflag & yflag & zflag){
            //          _rb.angularDrag = 1000;
            //     }
            // }

            // if (mainEngineValue > 0 || breakEngineValue > 0){
            //      if (localVelocity.y > 0.05)
            //     {
            //         speedAlignerValue2 = -1;
            //     }
            //     else if (localVelocity.y < -0.05)
            //     {
            //         speedAlignerValue2 = 1;
            //     }
            //     else if( localVelocity.y != 0){
            //         speedAlignerValue2 = 0;                    
            //     }

            //      if (localVelocity.x > 0.05)
            //     {
            //         speedAlignerValue = -1;
            //     }
            //     else if (localVelocity.x < -0.05)
            //     {
            //         speedAlignerValue = 1;
            //     }
            //     else if( localVelocity.x != 0){
            //         speedAlignerValue = 0;                
            //     }
            // }
            
            // _rb.AddForce(transform.right*speedAlignerValue*10000);
            // _rb.AddForce(transform.up*speedAlignerValue2*10000);
            // _rb.AddForce(-transform.forward * overridemainEngineValue * 10000);
            // _rb.AddForce(transform.forward * overridebreakEngineValue * 10000);
            // _rb.AddForce(transform.up * overridetakeOffEngineValue * 10000);
            // _rb.AddForce(-transform.up * overridelandingEngineValue * 10000);
            

            _rb.AddTorque(transform.right * Math.Clamp(overridezxRotationValue.y, -1.0f, 0f) * -1 * 10000);
            _rb.AddTorque(-transform.right * Math.Clamp(overridezxRotationValue.y, 0.0f, 1.0f) * 10000);
            _rb.AddTorque(transform.forward * Math.Clamp(overridezxRotationValue.x, 0.0f, 1.0f) * 10000);
            _rb.AddTorque(-transform.forward * Math.Clamp(overridezxRotationValue.x, -1.0f, 0f) * -1 * 10000);
            _rb.AddTorque(-childShip.forward * overrideleftYRotationValue * 10000);
            _rb.AddTorque(childShip.forward * overriderightYRotationValue * 10000);
            
            
        }

        _rb.AddForce(_gF.GetTotalFieldForceForBody(transform.position), ForceMode.Acceleration);
    } 

    void Update()
    {
        energyObject.UpdateEnergy(Time.deltaTime * (overridemainEngineValue + overridebreakEngineValue + overridetakeOffEngineValue + overridelandingEngineValue + Math.Abs(overridezxRotationValue.y) + Math.Abs(overridezxRotationValue.x) + overrideleftYRotationValue + overriderightYRotationValue) / 8);
    }

    public void SetupGravityField(GravityField gravity)
    {
        _gF = gravity;
    }

    public void SetupEnterShipArea(){
        
    }

    public void TurnOnTrails(){
        _tr1.emitting = true;
        _tr2.emitting = true;
        // _mtr1.enabled = true;
        // _mtr2.enabled = true;
    }

    public void TurnOffTrails(){
        _tr1.emitting = false;
        _tr2.emitting = false;
        // _mtr1.enabled = false;
        // _mtr2.enabled = false;
    }
}
