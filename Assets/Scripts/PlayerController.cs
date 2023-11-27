using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private InputActionMap _ingameControl;
    private float _healthPoints;
    [SerializeField] private InputActionAsset input;
    private InputAction movement;
    private InputAction jump;
    private InputAction wield;
    private InputAction shipRotation;
    private InputAction mainShipPropulsion;
    private InputAction takeOffPropulsion;
    private InputAction shipYRightRotation;
    private InputAction shipYLeftRotation;
    private InputAction brakePropulsion;

    public Image Health;
    public Transform _SphereT;
    [SerializeField] private Transform _tChild;
    [SerializeField] private Transform _tWeapon;
    [SerializeField] private ShipController _shipController;
    private Transform _t;
    private Rigidbody _rb;
    private Vector3 _rotationVector ;
    private Vector3 _toCenter;
    private GravityField _gF;
    private Vector3 _wieldVector;
    public Vector3 _lastForward;
    private float _rotationSpeed;
    private Quaternion _lastLocalRotation;
    private Quaternion _lastLocalWieldRotation;
    private bool onFloor;
    private bool notGamepad;
    private Camera cam;

    //private float airTimer;


    void Start()
    {   
        _healthPoints = 1.0f;
        onFloor = false;
        _t = transform;

        _ingameControl = input.FindActionMap("Ingame");
        movement = _ingameControl.FindAction("Movement");
        jump = _ingameControl.FindAction("Jump");
        wield = _ingameControl.FindAction("Wield");
        shipRotation = _ingameControl.FindAction("ShipRotation");
        mainShipPropulsion = _ingameControl.FindAction("MainShipPropulsion");
        takeOffPropulsion = _ingameControl.FindAction("TakeOffPropulsion");
        shipYRightRotation = _ingameControl.FindAction("ShipYRightRotation");
        shipYLeftRotation = _ingameControl.FindAction("ShipYLeftRotation");
        brakePropulsion = _ingameControl.FindAction("BrakePropulsion");

        // movement.Enable();
        // jump.Enable();
        // wield.Enable();
        shipRotation.Enable();
        mainShipPropulsion.Enable();
        takeOffPropulsion.Enable();
        shipYRightRotation.Enable();
        shipYLeftRotation.Enable();
        brakePropulsion.Enable();

        movement.performed += OnMove;
        movement.canceled += OnStop;
        jump.performed += OnJump;
        jump.canceled  += OnStopJump;
        wield.performed += OnWield;
        wield.canceled  += OnStopWield;
        shipRotation.performed += OnShipRotation;
        shipRotation.canceled += OnStopShipRotation;
        mainShipPropulsion.performed += OnMainShipPropulsion;
        mainShipPropulsion.canceled += OnStopMainShipPropulsion;
        takeOffPropulsion.performed += OnTakeOffPropulsion;
        takeOffPropulsion.canceled += OnStopTakeOffPropulsion;
        shipYRightRotation.performed += OnShipYRightRotation;
        shipYRightRotation.canceled += OnStopShipYRightRotation;
        shipYLeftRotation.performed += OnShipYLeftRotation;
        shipYLeftRotation.canceled += OnStopShipYLeftRotation;
        brakePropulsion.performed += OnBrakePropulsion;
        brakePropulsion.canceled += OnStopBrakePropulsion;

        _rb = GetComponent<Rigidbody>();
        _lastLocalRotation = _tChild.localRotation;
        _lastLocalWieldRotation = _tWeapon.localRotation;
        cam = Camera.main;
        _lastForward = _tWeapon.transform.forward;

    }
 
    
   

    void OnMove(InputAction.CallbackContext moveAction)
    {
        Vector2 inp = moveAction.ReadValue<Vector2>();
        _rotationVector = new Vector3(inp.x, 0, inp.y);
        _rotationVector.Normalize();
        _rotationSpeed = 100;
    }
    void OnStop(InputAction.CallbackContext moveAction)
    {
        _rotationSpeed = 0;
    }

    void OnJump(InputAction.CallbackContext jumpAction)
    {
        
        if (onFloor)
        {
            _rb.AddForce( -_toCenter*100 ,ForceMode.Impulse);  
            onFloor = false;
        }
        
    }

    void OnStopJump(InputAction.CallbackContext stopJumpAction)
    {
        
    }
    
    void OnWield(InputAction.CallbackContext wieldAction)
    {
        
        Vector2 inp = wieldAction.ReadValue<Vector2>();
        _wieldVector = new Vector3(inp.x, 0, inp.y);
        notGamepad = false;
    }

    void OnStopWield(InputAction.CallbackContext stopWieldAction)
    {
        notGamepad = true;
    }

    void OnShipRotation(InputAction.CallbackContext action){
        _shipController.zxRotationValue = action.ReadValue<Vector2>();
    }

    void OnStopShipRotation(InputAction.CallbackContext action){
        _shipController.zxRotationValue = Vector2.zero;
    }

    void OnMainShipPropulsion(InputAction.CallbackContext action){
        _shipController.mainEngineValue = action.ReadValue<float>();
    }

    void OnStopMainShipPropulsion(InputAction.CallbackContext action){
        _shipController.mainEngineValue = 0;
    }

    void OnTakeOffPropulsion(InputAction.CallbackContext action){
        _shipController.takeOffEngineValue = 1.0f;
    }

    void OnStopTakeOffPropulsion(InputAction.CallbackContext action){
        _shipController.takeOffEngineValue = 0.0f;
    }

    void OnShipYRightRotation(InputAction.CallbackContext action){
        _shipController.rightYRotationValue = action.ReadValue<float>();
    
    }

    void OnStopShipYRightRotation(InputAction.CallbackContext action){
        _shipController.rightYRotationValue = 0;
    }

    void OnShipYLeftRotation(InputAction.CallbackContext action){
         _shipController.leftYRotationValue = action.ReadValue<float>();
    
    }

    void OnStopShipYLeftRotation(InputAction.CallbackContext action){
        _shipController.leftYRotationValue = 0;
    }

    void OnBrakePropulsion(InputAction.CallbackContext action){
        _shipController.breakEngineValue = action.ReadValue<float>();
    }

    void OnStopBrakePropulsion(InputAction.CallbackContext action){
        _shipController.breakEngineValue = 0;
    }
    public void FixedUpdate()
    {
        _lastForward = _tWeapon.transform.forward;
        
        // _tWeapon.position = transform.position;

        Vector3 aux = cam.WorldToScreenPoint(_t.position);
        _wieldVector = new Vector3(Mouse.current.position.x.ReadValue() - aux.x,0,Mouse.current.position.y.ReadValue() - aux.y)  ;
        _wieldVector.Normalize();
          
       // print(cam.WorldToScreenPoint(_t.position));
        //print(_rotationVector);
        //_toCenter = (_SphereT.position - _t.position).normalized;
        _rb.AddForce(_gF.GetTotalFieldForceForBody(_t.position),ForceMode.Acceleration);
        //_rb.AddForce( _t.localToWorldMatrix.MultiplyVector(_rotationVector)*_rotationSpeed*2);

        if (_rb.velocity.magnitude > 10)
        {
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 10);
        }
        
        //_t.rotation = Quaternion.LookRotation( Vector3.ProjectOnPlane(_t.forward,-_toCenter).normalized,-_toCenter);

        // _tChild.localRotation = Quaternion.Slerp(Quaternion.LookRotation(_rotationVector, Vector3.up),_lastLocalRotation,0.8f);

        _lastLocalRotation = _tChild.localRotation;
        
        _tWeapon.localRotation = Quaternion.Slerp(Quaternion.LookRotation(_wieldVector, Vector3.up),_lastLocalWieldRotation,0.8f);
        //_tWeaponReversed.localRotation = _tWeapon.localRotation;
        //_tWeaponReversed.transform.Rotate(new Vector3(0,0,-180));

        _lastLocalWieldRotation = _tWeapon.localRotation;

        
 
    }


    private void UpdateHealth(){
       Health.transform.localScale = new Vector3(_healthPoints,1,1);
    }
    private void OnCollisionEnter(Collision col)
    {   
        if(col.gameObject.tag == "Planet"){
            onFloor = true;
        }else if(col.gameObject.tag == "Projectile"){
            if(_healthPoints > 0){
                _healthPoints-= 0.1f;
                UpdateHealth();
            }
            
        }
        
        
    }

    public void SetupGravityField(GravityField gravity){
        _gF = gravity;
        _shipController.SetupGravityField(gravity);
    }

   
}
