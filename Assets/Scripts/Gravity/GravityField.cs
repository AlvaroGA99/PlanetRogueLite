using System.Collections.Generic;
using UnityEngine;

public class GravityField 
{
    List<GravityBody> gravityBodies;

    public GravityField(){
        gravityBodies = new List<GravityBody>();
    }
    public void AddGravityBody(GravityBody body){
        gravityBodies.Add(body);
    }
    public Vector3 GetTotalFieldForceForBody(Vector3 pos){
        Vector3 total = new Vector3();
        foreach(GravityBody p in gravityBodies){
            Vector3 u = p.position.position - pos;
            float r = u.sqrMagnitude;
            u = u.normalized;
            total += u*p.mass / r;
         }
         return total;
    }
}
