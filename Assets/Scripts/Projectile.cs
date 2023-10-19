using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody _rb;
    private Vector3 _savedVector;

    private Transform _redirectTransform;

    private Transform _enemySourceTransform;

    ObjectPool<Projectile> _projPool;

    private bool colliding;
    void Start()
    {   
        colliding = false;
        _rb = GetComponent<Rigidbody>();
        Weapon.OnReleaseShot += TriggerForce;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Transform redirect, ObjectPool<Projectile> pool){
        _redirectTransform = redirect;
        _projPool = pool;
        
    }

    public void InitEnemySource(Transform enemy){
        _enemySourceTransform = enemy;
    }

    private void FixedUpdate()
    {   
        _rb.AddForce(_redirectTransform.position - transform.position , ForceMode.Acceleration);
        _rb.AddForce(-transform.position.normalized*50, ForceMode.Acceleration);
    }
    
    private void OnTriggerEnter(Collider other){
        colliding = true;
        _savedVector = other.transform.forward + other.transform.up;
    }

     private void OnTriggerExit(Collider other){
        colliding = false;
    }

    private void TriggerForce(int force){
        print("ACTIVADO");
        if(colliding){
             _rb.AddForce((_savedVector)*force,ForceMode.Impulse);
             _redirectTransform = _enemySourceTransform;
             colliding = false;
        }
       
    }

    void OnCollisionEnter(){
        _projPool.Release(this);
    }
}
