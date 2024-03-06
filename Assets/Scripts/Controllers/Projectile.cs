using System;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    public static event Action OnDestroyEnemy;
    private Rigidbody _rb;
    private GravityField _gF;

    ObjectPool<Projectile> _projPool;

    private bool colliding;
    void Start()
    {

        colliding = false;

    }
    private void OnDisable(){
        colliding = false;
    }

    public void Init( ObjectPool<Projectile> pool, GravityField gf)
    {
        _rb = GetComponent<Rigidbody>();
        _gF = gf;
        _projPool = pool;

    }

    public void Reset()
    {
        _rb.velocity = new Vector3(0, 0, 0);
        _rb.angularVelocity = new Vector3(0, 0, 0);
    }

    private void FixedUpdate()
    {
        _rb.AddForce(_gF.GetTotalFieldForceForBody(transform.position), ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        colliding = true;
    }

    private void OnTriggerExit(Collider other)
    {
        colliding = false;
    }

    void OnCollisionEnter(Collision other)
    {   
        _projPool.Release(this);
    }
}
