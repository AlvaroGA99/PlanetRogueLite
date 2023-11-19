using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody
{
    private Vector3 _frameDragVector = Vector3.zero;

    public float mass;
    public Vector3 position;

    public GravityBody(Vector3 pos,float bodyMass){
        position = pos;
        mass = bodyMass;
    }
    
    public GravityBody(Vector3 pos,Vector3 frameDrag){
        position = pos;
        _frameDragVector = frameDrag;
    }
}
