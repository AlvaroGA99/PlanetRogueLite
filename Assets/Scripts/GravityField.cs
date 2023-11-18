using System.Collections.Generic;
using UnityEngine;

public class GravityField 
{
    // Start is called before the first frame update
    List<Planet> gravityBodies;

    public GravityField(){
        gravityBodies = new List<Planet>();
    }
    public void AddGravityBody(Planet planet){
        gravityBodies.Add(planet);
    }
    public Vector3 GetTotalFieldForceForBody(Vector3 pos){
        Vector3 total = new Vector3();
        foreach(Planet p in gravityBodies){
            Vector3 u = p.transform.position - pos;
            float r = u.sqrMagnitude;
            u = u.normalized;
            total += u*p.mass / r;
         }
         return total;
    }
}
