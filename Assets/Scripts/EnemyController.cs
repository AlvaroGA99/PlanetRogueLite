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

        shouldSpawn = true;
        
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

        if (timer > 2 )
        {   
            if(shouldSpawn){
            Projectile projRef = _projPool.Get();

            projRef.transform.position = _t.position +(char_T.position - _t.position).normalized;

            UnityEngine.Debug.DrawLine(_t.position,projRef.transform.position,Color.red,2);
            projRef.InitEnemySource(_t);
            //projRef.Stop();
            projRef.GetComponent<Rigidbody>().AddForce(Vector3.ProjectOnPlane((char_T.position - _t.position),-_t.up)*2 + _t.up*4,ForceMode.Impulse);

            timer = 0;
            }


        }

        timer += Time.deltaTime;
    }
}
