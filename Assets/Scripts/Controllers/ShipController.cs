using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetProperties;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
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
    public Energy energyObject;
    public float speed;
    public bool onShip = true;
    public Vector3 lastVelocity { get; private set; }
    public event Action OnExitAtmosphere;
    public event Action<GameObject> OnEnterAtmosphere;

    private Rigidbody _rb;
    private GravityField _gF;
    [SerializeField] private RectTransform _uiEnterShip;
    [SerializeField]private MeshRenderer _mtr1;
    [SerializeField]private MeshRenderer _mtr2;
    [SerializeField] private Transform childShip;
    private Vector2 overridezxRotationValue;
    private float overriderightYRotationValue;
    private float overrideleftYRotationValue;
    private float overridemainEngineValue;
    private float overridebreakEngineValue;
    private float overridetakeOffEngineValue;
    private float overridelandingEngineValue;


    void Awake(){
        
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        speed = 2;

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
            
            if(speed > 0.01){
                _rb.velocity = -transform.forward*speed;  
            }
            _rb.angularDrag = 10;
            
            speed += overridemainEngineValue/5;
            speed -= overridebreakEngineValue/5;
            if (speed < 0){
                speed = 0;
            }
            
            _rb.AddTorque(transform.right * Math.Clamp(overridezxRotationValue.y, -1.0f, 0f) * -1 * 10000);
            _rb.AddTorque(-transform.right * Math.Clamp(overridezxRotationValue.y, 0.0f, 1.0f) * 10000);
            _rb.AddTorque(transform.forward * Math.Clamp(overridezxRotationValue.x, 0.0f, 1.0f) * 10000);
            _rb.AddTorque(-transform.forward * Math.Clamp(overridezxRotationValue.x, -1.0f, 0f) * -1 * 10000);
            _rb.AddTorque(-childShip.forward * overrideleftYRotationValue * 10000);
            _rb.AddTorque(childShip.forward * overriderightYRotationValue * 10000);
            _rb.AddForce(childShip.forward*overridetakeOffEngineValue*6000);
            _rb.AddForce(-childShip.forward*overridelandingEngineValue*6000);
            
        }

        _rb.AddForce(_gF.GetTotalFieldForceForBody(transform.position), ForceMode.Acceleration);
    } 

    void Update()
    {   _uiEnterShip.transform.LookAt(Camera.main.transform,Camera.main.transform.up);
        energyObject.UpdateEnergy(Time.deltaTime * (overridemainEngineValue + overridebreakEngineValue + overridetakeOffEngineValue + overridelandingEngineValue + Math.Abs(overridezxRotationValue.y) + Math.Abs(overridezxRotationValue.x) + overrideleftYRotationValue + overriderightYRotationValue) / 8);
    }

    public void SetupGravityField(GravityField gravity)
    {
        _gF = gravity;
    }

    public void SetupEnterShipArea(){
        
    }


    public void OnCollisionStay(Collision other){
        
        
        if(!(breakEngineValue >0 || mainEngineValue > 0))
            speed *= 0.7f;
        energyObject.energy -= speed*speed/1000;
    }

    private void OnTriggerStay(Collider col){
        if(col.gameObject.tag == "Ring" && energyObject.energy > 0){
            
                energyObject.UpdateEnergy(0.1f);
                if((transform.position - col.gameObject.transform.position).sqrMagnitude < 1200 && onShip){
                GameManager.OnReload?.Invoke();
            
            }
            
        }
    }

    private void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Projectile" ){
            energyObject.UpdateEnergy(-3);
            Destroy(col.gameObject);
        }else if(col.gameObject.tag == "Planet"){
            OnEnterAtmosphere?.Invoke(col.gameObject);
        }else if(col.gameObject.tag == "Sun"){
                energyObject.SetToZero();
            
            
        }else if(col.gameObject.tag == "Fluid"){
            Planet p = col.transform.parent.gameObject.GetComponent<Planet>();
            if(p!=null){
                if(p.fluidPropertie == PlanetLayerElement.Magma){
                    energyObject.SetToZero();
                }
            }
        }
    }

    private void OnTriggerExit(Collider col){
        if(col.gameObject.tag == "Planet"){
            OnExitAtmosphere?.Invoke();
        }
    }
}
