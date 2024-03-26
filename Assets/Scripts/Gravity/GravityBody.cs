using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class GravityBody
{
    [SerializeField]
    public float mass;
    [SerializeField]
    public Transform position;

    public GravityBody(Transform pos,float bodyMass){
        position = pos;
        mass = bodyMass;
    }
    
}
