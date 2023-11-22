using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Projectile _projPrefab;

    [SerializeField] private bool shouldSpawn;
    [SerializeField] private Rigidbody _rb;
    ObjectPool<Projectile> _projPool;

    private Transform char_T;
    //private Transform _SphereT;
    private Transform _t;
    private bool jumping;
    private float timer;
    private GravityField _gF;

    private LayerMask _wallMask;

    private enum State
    {
        AttackState,
        MoveState,
        AttackMoveState
    }
    private State BehaviourState;

    void Awake()
    {
        jumping = false;
        timer = 0;
        shouldSpawn = true;
        _rb = GetComponent<Rigidbody>();
        _t = transform;
    }

    void OnEnable()
    {
        timer = 0;
    }

    public void Init(ObjectPool<Projectile> pool, Transform charTransform, LayerMask wallMask)
    {
        _projPool = pool;
        char_T = charTransform;
        //_SphereT = sphereTransform;
        _wallMask = wallMask;
        BehaviourState = CheckBehaviourState((char_T.position - _t.position).sqrMagnitude);
    }
    private void FixedUpdate()
    {
        //Vector3 toCenter = _t.position - _SphereT.position;
        Vector3 betweenThisAndChar = char_T.position - _t.position;
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, transform.position).normalized, transform.position);
        //_rb.AddForce(-toCenter.normalized * 115);
        Vector3 moveVector = Vector3.ProjectOnPlane(betweenThisAndChar, -_t.up).normalized * 200;
        _t.rotation = Quaternion.LookRotation(moveVector, _t.up);
        float sqrDistance = betweenThisAndChar.sqrMagnitude;
        Vector3 launchVector = betweenThisAndChar.normalized*1.5f;
        float launchVectorMag = betweenThisAndChar.magnitude * 3;

        if (!jumping)
        {

            RaycastHit hit;
            if (Physics.Raycast(_t.position, _t.forward, out hit, 2.0f,_wallMask))
            {   
                // Vector3 vec = (hit.point - _SphereT.position).normalized;
                // vec = (vec*1.01f )*44 - _t.position;
                // float vecMag = vec.magnitude;
                // vec = vec.normalized*4;
                //_rb.AddForce((vec + _t.up * 4).normalized * vecMag, ForceMode.Impulse);
                //_rb.AddForce(_t.up * 1000, ForceMode.Impulse);
                //_rb.AddForce(_t.up * 100 + _t.forward*2, ForceMode.Impulse);
                jumping = true;
                StartCoroutine("Jumping");
            }
            else
            {
                if (BehaviourState == State.AttackState)
                {
                    if (timer > 3)
                    {
                        if (shouldSpawn)
                        {
                            LaunchProjectile(launchVector, launchVectorMag);
                            timer = 0;
                        }
                    }

                    BehaviourState = CheckBehaviourState(sqrDistance);
                    if (BehaviourState != State.AttackState)
                    {
                        timer = 0;
                    }
                }
                else if (BehaviourState == State.MoveState)
                {
                    
                    _rb.AddForce(moveVector);
                    BehaviourState = CheckBehaviourState(sqrDistance);
                    timer = 0;
                }
                else
                {
                    _rb.AddForce(moveVector);
                    if (timer > 3)
                    {
                        if (shouldSpawn)
                        {
                            LaunchProjectile(launchVector, launchVectorMag);
                            timer = 0;
                        }
                    }
                    BehaviourState = CheckBehaviourState(sqrDistance);
                    if (BehaviourState != State.AttackMoveState)
                    {
                        timer = 0;
                    }
                }

                timer += Time.fixedDeltaTime;
            }


        }



    }

    private State CheckBehaviourState(float sqrMagnitude)
    {
        if (sqrMagnitude < 225)
        {
            return State.AttackState;
        }
        else if (sqrMagnitude >= 225 && sqrMagnitude < 576)
        {
            return State.AttackMoveState;
        }
        else
        {
            return State.MoveState;
        }
    }

    private void LaunchProjectile(Vector3 launchVector, float launchVectorMag)
    {
        Projectile projRef = _projPool.Get();
        projRef.transform.position = _t.position + _t.up * 2;
        projRef.InitEnemySource(_t);
        projRef.GetComponent<Rigidbody>().AddForce((launchVector + _t.up * 4).normalized * launchVectorMag, ForceMode.Impulse);
        UnityEngine.Debug.DrawLine(_t.position, _t.position + (launchVector + _t.up * 4).normalized * launchVectorMag, Color.red, 2);
    }

    private IEnumerator Jumping()
    {
        float time = 0;
        while (time < 3)
        {
            time += Time.deltaTime;
            yield return null;
        }
        jumping = false;
        timer = 0;
    }
}
