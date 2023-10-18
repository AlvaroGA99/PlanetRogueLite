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

    [SerializeField] private bool shouldSpawn;

    ObjectPool<Projectile> _projPool;
    
    private Transform char_T;
    private Transform _t;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
      
        
    }

    void Awake(){

        timer = 0;
        
        char_T = GameObject.Find("Char_position").transform;

        _t = transform;
    }

    // Update is called once per frame
    void Update()
    {   

        
    }

    void OnEnable(){
        timer = 0;
    }
    

    public void Init(ObjectPool<Projectile> pool){
        _projPool = pool;
    }
    private void FixedUpdate()
    {   

        transform.rotation = Quaternion.LookRotation( Vector3.ProjectOnPlane(transform.forward,transform.position).normalized,transform.position);
        //transform.position += transform.forward/10;

        if (timer > 1 )
        {   
            if(shouldSpawn){
            Projectile projRef = Instantiate(_projPrefab, _t.position + _t.forward, Quaternion.identity );

            projRef.Init(char_T,_t);

            projRef.GetComponent<Rigidbody>().AddForce((char_T.position - _t.position)*5,ForceMode.Impulse);

            timer = 0;
            }


        }

        timer += Time.deltaTime;
    }
}
