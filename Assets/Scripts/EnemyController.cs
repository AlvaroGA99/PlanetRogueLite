using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    private Transform _SphereT;
    private Transform _t;
    private float timer;

    private enum State
    {
        AttackState,
        MoveState,
        AttackMoveState
    }
    private State BehaviourState;

    void Awake()
    {
        timer = 0;
        shouldSpawn = true;
        _rb = GetComponent<Rigidbody>();
        _t = transform;
    }

    void OnEnable()
    {
        timer = 0;
    }
    
    public void Init(ObjectPool<Projectile> pool, Transform charTransform,Transform sphereTransform)
    {
        _projPool = pool;
        char_T = charTransform;
        _SphereT = sphereTransform;

        BehaviourState = CheckBehaviourState((char_T.position - _t.position).sqrMagnitude);
    }
    private void FixedUpdate()
    {
        Vector3 toCenter = _t.position - _SphereT.position;
        Vector3 betweenThisAndChar = char_T.position - _t.position;
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, transform.position).normalized, transform.position);
        _rb.AddForce(-toCenter.normalized*115);
        float sqrDistance = betweenThisAndChar.sqrMagnitude;
        Vector3 launchVector = betweenThisAndChar.normalized;
        float launchVectorMag = betweenThisAndChar.magnitude*4;

        if (BehaviourState == State.AttackState)
        {
            if (timer > 3)
            {
                if (shouldSpawn)
                {
                    LaunchProjectile(launchVector,launchVectorMag);
                    timer = 0;
                }
            }

            BehaviourState = CheckBehaviourState(sqrDistance);
            if (BehaviourState != State.AttackState){
                timer = 0;
            }
        }
        else if (BehaviourState == State.MoveState)
        {   
            
            BehaviourState = CheckBehaviourState(sqrDistance);
            timer = 0;
        }
        else
        {   
            _rb.AddForce(Vector3.ProjectOnPlane(betweenThisAndChar, -_t.up).normalized*200);
            if (timer > 3)
            {
                if (shouldSpawn)
                {
                    LaunchProjectile(launchVector,launchVectorMag);
                    timer = 0;
                }
            }
            BehaviourState = CheckBehaviourState(sqrDistance);
            if (BehaviourState != State.AttackMoveState){
                timer = 0;
            }
        }

        timer += Time.deltaTime;
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

    private void LaunchProjectile(Vector3 launchVector, float launchVectorMag){
        Projectile projRef = _projPool.Get();
        projRef.transform.position = _t.position + _t.up*2;
        projRef.InitEnemySource(_t);
        projRef.GetComponent<Rigidbody>().AddForce((launchVector + _t.up*4).normalized*launchVectorMag, ForceMode.Impulse);
        UnityEngine.Debug.DrawLine(_t.position, _t.position + (launchVector + _t.up*4).normalized*launchVectorMag, Color.red, 2);
    }

    // private IEnumerator CheckFrontObstacles(Vector3 launchVector, float launchVectorMag){
    //     while(!Physics.Raycast(_t.position,_t.forward,5)){
    //         yield return new WaitForFixedUpdate();
    //     }
    //     //hacer jump
    //     yield return new WaitForFixedUpdate();
        
    // }
}
