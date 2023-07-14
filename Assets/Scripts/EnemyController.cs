using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject _projPrefab;
    
    
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (timer > 1 )
        {
            var projRef = Instantiate(_projPrefab, transform  );
            
            projRef.GetComponent<Rigidbody>().AddForce(transform.forward);

            timer = 0;

        }

        timer += Time.deltaTime;
    }
}
