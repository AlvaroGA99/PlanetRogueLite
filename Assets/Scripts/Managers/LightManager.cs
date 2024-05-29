using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{   
    [SerializeField] private Transform _mainTarget;

    private Transform _target;
    [SerializeField] private Light _mainLight;
    private int _layer;

    void Awake()
    {   
        _target = _mainTarget;
        _layer = 9;
    }

    void Update()
    {   
        _mainLight.transform.LookAt(_target);
       
    }
    
    public void Target(Transform target){
        _target = target;
    }

    public void ResetTarget(){
        _target = _mainTarget;
    }
}
