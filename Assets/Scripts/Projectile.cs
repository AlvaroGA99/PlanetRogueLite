using System;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update

    public static event Action OnDestroyEnemy;

    private Rigidbody _rb;
    private Vector3 _savedVector;
    private Transform _redirectTransform;
    private Transform _enemySourceTransform;
    private Transform _SphereT;
    private GravityField _gF;

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

    public void Init(Transform redirect, ObjectPool<Projectile> pool,Transform sphereTransform)
    {
        _rb = GetComponent<Rigidbody>();
        _redirectTransform = redirect;
        _projPool = pool;
        _SphereT = sphereTransform;

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
        Vector3 helpForce = _redirectTransform.position - transform.position;
        _rb.AddForce(helpForce/helpForce.sqrMagnitude*40, ForceMode.Acceleration);
        _rb.AddForce(-transform.position.normalized * 50, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        colliding = true;
        _redirectTransform = other.transform;
        
    }

    private void OnTriggerExit(Collider other)
    {
        colliding = false;
    }

    private void TriggerForce(float force)
    {
        //print("ACTIVADO");
        if (colliding)
        {   
            _savedVector =  _enemySourceTransform.position - _redirectTransform.position;
            _savedVector = (_savedVector.normalized*1.5f + _redirectTransform.up*4).normalized*_savedVector.magnitude*3;
            _rb.velocity = new Vector3(0, 0, 0);
            _rb.angularVelocity = new Vector3(0, 0, 0);
            _rb.AddForce((_savedVector) * 1, ForceMode.Impulse);
            _redirectTransform = _enemySourceTransform;
            colliding = false;
        }

    }

    void OnCollisionEnter(Collision other)
    {   //print(other.gameObject.tag);
        if(other.gameObject.tag == "Enemy"){
             OnDestroyEnemy?.Invoke();
             
        }
        _projPool.Release(this);
    }
}
