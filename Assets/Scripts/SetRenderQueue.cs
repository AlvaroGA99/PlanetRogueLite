using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
    // Start is called before the first frame update

    public int queue;
    void Start()
    {
        GetComponent<Renderer>().material.renderQueue = queue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
