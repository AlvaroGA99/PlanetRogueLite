using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class GravityBody
{
    [SerializeField]
    private Vector3 _frameDragVector = Vector3.zero;
    [SerializeField]
    public float mass;
    [SerializeField]
    public Transform position;

    public GravityBody(Transform pos,float bodyMass){
        position = pos;
        mass = bodyMass;
    }
    
    public GravityBody(Transform pos,Vector3 frameDrag,float bodyMass){
        position = pos;
        _frameDragVector = frameDrag;
        mass = bodyMass;
    }
}
