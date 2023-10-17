using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Projectile _projPrefab;
    
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

        transform.rotation = Quaternion.LookRotation( Vector3.ProjectOnPlane(transform.forward,transform.position).normalized,transform.position);
        transform.position += transform.forward/10;

        if (timer > 1 )
        {
            Projectile projRef = Instantiate(_projPrefab, transform.position + transform.forward, Quaternion.identity );
            
            projRef.Init(GameObject.Find("Player").transform,transform);

            projRef.GetComponent<Rigidbody>().AddForce(transform.forward*5,ForceMode.Impulse);

            timer = 0;

        }

        timer += Time.deltaTime;
    }
}
