using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{   
    private Rigidbody _rb;

    [SerializeField]
    private Transform _mainEngine;
    [SerializeField]
    private Transform _takeOffEngine;
    [SerializeField]
    private Transform _brakeEngine;
    [SerializeField]
    private Transform _upZRotationEngine;
    [SerializeField]
    private Transform _downZRotationEngine;
    [SerializeField]
    private Transform _leftXRotationEngine;
    [SerializeField]
    private Transform _rightXRotationEngine;
    [SerializeField]
    private Transform _leftYRotationEngine;
    [SerializeField]
    private Transform _rightYRotationEngine;

    public Vector2 zxRotationValue;
    public float rightYRotationValue;
    public float leftYRotationValue;
    public float mainEngineValue;
    public float breakEngineValue;
    public bool takeOffEngineValue;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame

    void FixedUpdate(){
        _rb.AddForceAtPosition(_mainEngine.up,_mainEngine.position);
        _rb.AddForceAtPosition(_takeOffEngine.up,_takeOffEngine.position);
        _rb.AddForceAtPosition(_brakeEngine.up,_brakeEngine.position);
        _rb.AddForceAtPosition(_upZRotationEngine.up,_upZRotationEngine.position);
        _rb.AddForceAtPosition(_downZRotationEngine.up,_downZRotationEngine.position);
        _rb.AddForceAtPosition(_leftXRotationEngine.up,_leftXRotationEngine.position);
        _rb.AddForceAtPosition(_rightXRotationEngine.up,_rightXRotationEngine.position);
        _rb.AddForceAtPosition(_leftYRotationEngine.up,_leftYRotationEngine.position);
        _rb.AddForceAtPosition(_rightYRotationEngine.up,_rightYRotationEngine.position);
    }
}
