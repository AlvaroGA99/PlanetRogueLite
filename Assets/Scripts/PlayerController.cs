using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private InputActionMap _ingameControl;

    private Player a;

    [SerializeField] private InputActionAsset input;

    private InputAction movement;
    private InputAction jump;
    private InputAction wield;
    
    public Transform _SphereT;
    [SerializeField] private Transform _tChild;
    [SerializeField] private Transform _tWeapon;
    [SerializeField] private Transform _tWeaponReversed;
    private Transform _t;

    private Rigidbody _rb;
    
    private Vector3 _rotationVector ;
    private Vector3 _toCenter;
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
        onFloor = false;
        _t = transform;
        _ingameControl = input.FindActionMap("Ingame");
        movement = _ingameControl.FindAction("Movement");
        jump = _ingameControl.FindAction("Jump");
        wield = _ingameControl.FindAction("Wield");
        movement.Enable();
        jump.Enable();
        wield.Enable();
        movement.performed += OnMove;
        movement.canceled += OnStop;
        jump.performed += OnJump;
        jump.canceled  += OnStopJump;
        wield.performed += OnWield;
        wield.canceled  += OnStopWield;
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
    public void FixedUpdate()
    {
        _lastForward = _tWeapon.transform.forward;
        
        // _tWeapon.position = transform.position;

        Vector3 aux = cam.WorldToScreenPoint(_t.position);
        _wieldVector = new Vector3(Mouse.current.position.x.ReadValue() - aux.x,0,Mouse.current.position.y.ReadValue() - aux.y)  ;
        _wieldVector.Normalize();
          
       // print(cam.WorldToScreenPoint(_t.position));
        //print(_rotationVector);
        _toCenter = (_SphereT.position - _t.position).normalized;
        _rb.AddForce(_toCenter * 115);
        _rb.AddForce( _t.localToWorldMatrix.MultiplyVector(_rotationVector)*_rotationSpeed*2);

        if (_rb.velocity.magnitude > 10)
        {
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 10);
        }
        
        _t.rotation = Quaternion.LookRotation( Vector3.ProjectOnPlane(_t.forward,-_toCenter).normalized,-_toCenter);

        // _tChild.localRotation = Quaternion.Slerp(Quaternion.LookRotation(_rotationVector, Vector3.up),_lastLocalRotation,0.8f);

        _lastLocalRotation = _tChild.localRotation;
        
        _tWeapon.localRotation = Quaternion.Slerp(Quaternion.LookRotation(_wieldVector, Vector3.up),_lastLocalWieldRotation,0.8f);
        //_tWeaponReversed.localRotation = _tWeapon.localRotation;
        //_tWeaponReversed.transform.Rotate(new Vector3(0,0,-180));

        _lastLocalWieldRotation = _tWeapon.localRotation;

        
 
    }


    private void OnCollisionEnter(Collision col)
    {
        onFloor = true;
        
    }

   
}
