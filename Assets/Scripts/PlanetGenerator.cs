using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{   
    private GameObject[] _orbits;

    public GameObject planetPrefab;
    // Start is called before the first frame update
    void Start()
    {
        _orbits = new GameObject[6];
        float angle = 0.0f;
        for (int i = 0; i < _orbits.Length; i++)
        {   
            angle = Random.Range(0.66f*Mathf.PI,Mathf.PI/3);
            print(angle);
           _orbits[i] =  Instantiate(planetPrefab,transform.position + new Vector3(250*(i+1)*Mathf.Cos(angle),0,-250*(i+1)*Mathf.Sin(angle)), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
