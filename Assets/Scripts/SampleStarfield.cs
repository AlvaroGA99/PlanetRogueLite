using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SampleStarfield : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 _cameraWorldDir;
    System.Random random = new System.Random();
    void Start()
    {
       float x =  UnityEngine.Random.value;
       float y = UnityEngine.Random.value;

       float theta = x*math.PI*2;
       float phi = y*math.PI;

       Vector3 dir = new Vector3(math.sin(phi)*math.cos(theta),math.sin(phi)*math.sin(theta),math.cos(phi));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
