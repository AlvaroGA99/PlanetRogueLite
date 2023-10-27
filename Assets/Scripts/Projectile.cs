using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
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
        Weapon.OnReleaseShot += TriggerForce;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable(){
        colliding = false;
    }

    public void Init(Transform redirect, ObjectPool<Projectile> pool)
    {
        _rb = GetComponent<Rigidbody>();
        _redirectTransform = redirect;
        _projPool = pool;

    }

    public void InitEnemySource(Transform enemy)
    {
        _enemySourceTransform = enemy;
    }

    public void Reset(Transform redirect)
    {
        _redirectTransform = redirect;
        _rb.velocity = new Vector3(0, 0, 0);
        _rb.angularVelocity = new Vector3(0, 0, 0);
    }

    private void FixedUpdate()
    {
        _rb.AddForce(_redirectTransform.position - transform.position, ForceMode.Acceleration);
        _rb.AddForce(-transform.position.normalized * 50, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        colliding = true;
        _savedVector =  Vector3.ProjectOnPlane( _enemySourceTransform.position - _redirectTransform.position ,-other.transform.up) + other.transform.up*2;
    }

    private void OnTriggerExit(Collider other)
    {
        colliding = false;
    }

    private void TriggerForce(float force)
    {
        print("ACTIVADO");
        if (colliding)
        {
            _rb.velocity = new Vector3(0, 0, 0);
            _rb.angularVelocity = new Vector3(0, 0, 0);
            _rb.AddForce((_savedVector) * force, ForceMode.Impulse);
            _redirectTransform = _enemySourceTransform;
            colliding = false;
        }

    }

    void OnCollisionEnter()
    {
        _projPool.Release(this);
    }
}
