using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public event Action<GameObject> OnEnterAtmosphere;
    public event Action OnExitAtmosphere;
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
    private InputAction landingPropulsion;
    private InputAction cameraAction;
    private InputAction brakeAction;
    private InputAction speedAlignAction;

    private InputAction eject_enterShip;

    public Image Health;
    public Transform _SphereT;
    [SerializeField] private Transform _tChild;
    [SerializeField] private Transform _tHead;
    [SerializeField] private Transform _tWeapon;
    [SerializeField] private ShipController _shipController;

    [SerializeField] private LightManager _lM;
    private Transform _t;
    private Rigidbody _rb;
    private Vector3 _rotationVector ;
    private Vector3 _toCenter;
    private GravityField _gF;
    private Vector3 _wieldVector;
    private Vector3 _moveVector;
    public Vector3 _lastForward;
    private float _rotationSpeed;
    private Quaternion _lastLocalRotation;
    private Quaternion _lastLocalWieldRotation;
    private bool onFloor;
    private bool nearShip;
    private bool onShip = true;
    public Energy _energyObject;
    private float _jetpackProp;
    private Camera cam;

    public float camShakeIntesity;

    [SerializeField] private VelocityDirection _vD;

    private Vector2 rotateCamValue;

    private Vector2 offset;

    [SerializeField] GameObject characterLight;

    //private float airTimer;


    void Start()
    {   
        _healthPoints = 1.0f;
        onFloor = false;
        nearShip = true;
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
        landingPropulsion = _ingameControl.FindAction("LandingPropulsion");
        cameraAction = _ingameControl.FindAction("Camera");
        eject_enterShip = _ingameControl.FindAction("Eject_EnterShip");
        brakeAction = _ingameControl.FindAction("Brake");
        speedAlignAction = _ingameControl.FindAction("SpeedAlign");

        // movement.Enable();
        // jump.Enable();
        // wield.Enable();
        //shipRotation.Enable();
        //mainShipPropulsion.Enable();
        //takeOffPropulsion.Enable();
        //shipYRightRotation.Enable();
        //shipYLeftRotation.Enable();
        //brakePropulsion.Enable();

        eject_enterShip.Enable();

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
        landingPropulsion.performed += OnLandingPropulsion;
        landingPropulsion.canceled += OnStopLandingPropulsion;
        cameraAction.performed += OnCameraAction;
        cameraAction.canceled += OnStopCameraAction;
        eject_enterShip.performed += Eject;
        brakeAction.performed += OnBrake;
        brakeAction.canceled += OnStopBrake;
        speedAlignAction.performed += OnSpeedAlign;
        speedAlignAction.canceled += OnStopSpeedAlign;


        _rb = GetComponent<Rigidbody>();
        _lastLocalRotation = _tChild.localRotation;
        _lastLocalWieldRotation = _tWeapon.localRotation;
        cam = Camera.main;
        _lastForward = _tWeapon.transform.forward;
        camShakeIntesity = 0;

    }
 
    void Update(){
        //transform - offset
        cam.transform.localPosition = cam.transform.localPosition - new Vector3(offset.x,offset.y,0);
        cam.transform.RotateAround(transform.position,transform.up,rotateCamValue.x);
        //valRandom += 3*time.deltatime
        //if val random > tamtextura 
        //random 
        offset = new Vector2(Mathf.PerlinNoise(Time.time*1.7f,0)-0.5f,Mathf.PerlinNoise(0,Time.time*1.7f)-0.5f)*_vD.velocityMag/1.4f;
        cam.transform.localPosition = cam.transform.localPosition + new Vector3(offset.x,offset.y,0);
        //var xoffset = leerTexturaenvalorrandomx*intensidad;
        //var yoffset = leerTexturaenvalorrandomy*intensidad;
        //transform + offset
        
        // Vector3 axis = Vector3.Cross(transform.up,transform.position - cam.transform.position); //new Vector2(cam.transform.localPosition.x,cam.transform.localPosition.y).sqrMagnitude > 1)
        // if(axis.sqrMagnitude > 0 && rotateCamValue.y > 0){
        //     cam.transform.RotateAround(transform.position,Vector3.Cross(transform.up,transform.position - cam.transform.position),rotateCamValue.y);
        // }else if(axis.sqrMagnitude > 0 && rotateCamValue.y < 0)
        // {
        //     cam.transform.RotateAround(transform.position,Vector3.Cross(-transform.up,transform.position - cam.transform.position),math.abs(rotateCamValue.y));
        // }
        _energyObject.UpdateEnergy(Time.deltaTime*_jetpackProp/30);
    }
   
    void OnMove(InputAction.CallbackContext moveAction)
    {
        Vector2 inp = moveAction.ReadValue<Vector2>();
        _rotationVector = new Vector3(inp.x, 0, inp.y);
        _rotationVector.Normalize();
        _moveVector = Vector3.ProjectOnPlane(cam.transform.localToWorldMatrix.MultiplyVector(_rotationVector),-_toCenter);
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
            
            _rb.AddForce( -_toCenter*4 ,ForceMode.Impulse);  
            onFloor = false;
        }else{
            _jetpackProp = 10f;
        }
        
    }

    void OnStopJump(InputAction.CallbackContext stopJumpAction)
    {
        _jetpackProp = 0;
    }
    
    void OnWield(InputAction.CallbackContext wieldAction)
    {
        
        Vector2 inp = wieldAction.ReadValue<Vector2>();
        _wieldVector = new Vector3(inp.x, 0, inp.y);
        //notGamepad = false;
    }

    void OnStopWield(InputAction.CallbackContext stopWieldAction)
    {
        //notGamepad = true;
    }

    void OnShipRotation(InputAction.CallbackContext action){
        _shipController.zxRotationValue = action.ReadValue<Vector2>();
    }

    void OnStopShipRotation(InputAction.CallbackContext action){
        _shipController.zxRotationValue = Vector2.zero;
    }

    void OnMainShipPropulsion(InputAction.CallbackContext action){
        _shipController.mainEngineValue = action.ReadValue<float>();
        _shipController.TurnOnTrails();
    }

    void OnStopMainShipPropulsion(InputAction.CallbackContext action){
        _shipController.mainEngineValue = 0;
        _shipController.speedAlignerValue= 0;
        _shipController.speedAlignerValue2= 0;
        _shipController.TurnOffTrails();
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
        _shipController.speedAlignerValue= 0;
        _shipController.speedAlignerValue2= 0;

    }

    void OnLandingPropulsion(InputAction.CallbackContext action){
        _shipController.landingEngineValue = 1.0f;
        
    }

    void OnStopLandingPropulsion(InputAction.CallbackContext action){
        _shipController.landingEngineValue = 0.0f;
    }

    void OnCameraAction(InputAction.CallbackContext action){
        rotateCamValue = action.ReadValue<Vector2>();
        //cam.transform.RotateAround(transform.position,transform.up,input.x);
    }

    void OnStopCameraAction(InputAction.CallbackContext action){
        rotateCamValue = Vector2.zero;
    }

    private void Eject(InputAction.CallbackContext action){
        if(onShip){
            characterLight.SetActive(true);
            camShakeIntesity = 0.0f;
            _rb.isKinematic = false;
            _t.position -= new Vector3(5f,0,0);
            _t.parent = null;
            onShip = false;
            GetComponent<BoxCollider>().enabled = true;
            EnablePlayerController();
            DisableShipController();
            print("EJECT");
        }else if(nearShip){
            characterLight.SetActive(false);
            camShakeIntesity = 1.0f;
            onShip = true;
            _rb.isKinematic = true;
            _t.parent = _shipController.transform;
            _t.localPosition = new Vector3(-0.09f,0.316f,-1.659f);
            _t.localRotation = Quaternion.identity;//Quaternion.LookRotation(Vector3.forward,Vector3.up);
            
            onShip = true;
            GetComponent<BoxCollider>().enabled = false;
            DisablePlayerController();
            EnableShipController();
        }
       
    }

    private void OnBrake(InputAction.CallbackContext action){
        _shipController.breakActivated = true;
    }

    private void OnStopBrake(InputAction.CallbackContext action){
        _shipController.breakActivated = false;
    }

    private void OnSpeedAlign(InputAction.CallbackContext action){
        _shipController.speedAligner = true;
    }

    private void OnStopSpeedAlign(InputAction.CallbackContext action){
        _shipController.speedAligner = false;
        _shipController.speedAlignerValue = 0;
    }


    public void DisablePlayerController(){
        movement.Disable();
        jump.Disable();
        wield.Disable();
    }

    public void EnablePlayerController(){
        movement.Enable();
        jump.Enable();
        wield.Enable();
    }

    public void DisableShipController(){
        shipRotation.Disable();
        mainShipPropulsion.Disable();
        takeOffPropulsion.Disable();
        shipYRightRotation.Disable();
        shipYLeftRotation.Disable();
        brakePropulsion.Disable();
        landingPropulsion.Disable();
        brakeAction.Disable();
        speedAlignAction.Disable();
    }

    public void EnableShipController(){
        shipRotation.Enable();
        mainShipPropulsion.Enable();
        takeOffPropulsion.Enable();
        shipYRightRotation.Enable();
        shipYLeftRotation.Enable();
        brakePropulsion.Enable();
        landingPropulsion.Enable();
        brakeAction.Enable();
        speedAlignAction.Enable();
    }

    public void EnableCameraController(){
        cameraAction.Enable();
        rotateCamValue = Vector2.zero;
    }

    public void DisableCameraController(){
        cameraAction.Disable();
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
        _toCenter = _gF.GetTotalFieldForceForBody(_tChild.position);
        _rb.AddForce(_toCenter,ForceMode.Acceleration);
           
        
        if(onFloor){
            _rb.AddForce( _moveVector.normalized*_rotationSpeed);
        }
        
        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 10);
        
        if(!onShip){
            _t.rotation = Quaternion.LookRotation( Vector3.ProjectOnPlane(_t.forward,-_toCenter).normalized,-_toCenter);
            _tChild.rotation = Quaternion.Slerp(Quaternion.LookRotation(Vector3.ProjectOnPlane(_moveVector,_toCenter).normalized, -_toCenter),_lastLocalRotation,0.8f);
        }
       

        _lastLocalRotation = _tChild.rotation;
        
        _tWeapon.localRotation = Quaternion.Slerp(Quaternion.LookRotation(_wieldVector, Vector3.up),_lastLocalWieldRotation,0.8f);
        _lastLocalWieldRotation = _tWeapon.localRotation;

        if(_energyObject.energy > 0){
            _rb.AddForce((_tChild.up+_tChild.forward)*_jetpackProp*4);
        }
        
 
    }


    private void UpdateHealth(){
       Health.transform.localScale = new Vector3(_healthPoints,1,1);
    }
    private void OnCollisionEnter(Collision col)
    {   
        if(col.gameObject.tag == "Planet"){
            onFloor = true;
            _rotationSpeed = 100;
        }else if(col.gameObject.tag == "Projectile"){
            if(_healthPoints > 0){
                _healthPoints-= 0.1f;
                UpdateHealth();
            }
            
        }
    }

    private void OnTriggerEnter(Collider col)
    {   
        if(col.gameObject.tag == "Planet"){
            _lM.Target(col.transform);
            col.gameObject.layer = 0;
            OnEnterAtmosphere?.Invoke(col.gameObject);
        }else if(col.gameObject.tag == "Ship"){
            nearShip = true;
        }
            
    }

    private void OnTriggerExit(Collider col)
    {   
        if(col.gameObject.tag == "Planet"){
            _lM.ResetTarget();
            col.gameObject.layer = 9;
            OnExitAtmosphere?.Invoke();
        }else if(col.gameObject.tag == "Projectile"){

        }else if(col.gameObject.tag == "Energy"){
        }else if(col.gameObject.tag == "Ship"){
            nearShip = false;
        }
            
    }

    public void SetupGravityField(GravityField gravity){
        _gF = gravity;
        _shipController.SetupGravityField(gravity);
    }

    public void SetupHeadLook(){
        //_tHead.parent = Camera.main.transform;
    }

}
