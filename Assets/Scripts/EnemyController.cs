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
    // Start is called before the first frame update
    void Start()
    {


    }

    void Awake()
    {

        timer = 0;

        shouldSpawn = true;

        //char_T = GameObject.Find("Char_position").transform;
        _rb = GetComponent<Rigidbody>();

        _t = transform;
    }

    // Update is called once per frame
    void Update()
    {


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

        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, transform.position).normalized, transform.position);
        _rb.AddForce((_SphereT.position - _t.position).normalized*115);
        float sqrDistance = (char_T.position - _t.position).sqrMagnitude;

        if (BehaviourState == State.AttackState)
        {
            if (timer > 2)
            {
                if (shouldSpawn)
                {
                    Projectile projRef = _projPool.Get();
                    projRef.transform.position = _t.position + (char_T.position - _t.position).normalized*2;
                    UnityEngine.Debug.DrawLine(_t.position, projRef.transform.position, Color.red, 2);
                    projRef.InitEnemySource(_t);
                    projRef.GetComponent<Rigidbody>().AddForce(Vector3.ProjectOnPlane((char_T.position - _t.position), -_t.up) * 4 + _t.up * 2, ForceMode.Impulse);
                    timer = 0;
                }
            }

            BehaviourState = CheckBehaviourState((char_T.position - _t.position).sqrMagnitude);
            if (BehaviourState != State.AttackState){
                timer = 0;
            }
        }
        else if (BehaviourState == State.MoveState)
        {   
            
            BehaviourState = CheckBehaviourState((char_T.position - _t.position).sqrMagnitude);
            timer = 0;
        }
        else
        {   
            _rb.AddForce(Vector3.ProjectOnPlane((char_T.position - _t.position), -_t.up).normalized*200);
            if (timer > 2)
            {
                if (shouldSpawn)
                {
                    Projectile projRef = _projPool.Get();
                    projRef.transform.position = _t.position + (char_T.position - _t.position).normalized*2;
                    UnityEngine.Debug.DrawLine(_t.position, projRef.transform.position, Color.red, 2);
                    projRef.InitEnemySource(_t);
                    projRef.GetComponent<Rigidbody>().AddForce(Vector3.ProjectOnPlane((char_T.position - _t.position), -_t.up) * 4 + _t.up * 2, ForceMode.Impulse);
                    timer = 0;
                }
            }
            BehaviourState = CheckBehaviourState((char_T.position - _t.position).sqrMagnitude);
            if (BehaviourState != State.AttackMoveState){
                timer = 0;
            }
        }

        timer += Time.deltaTime;
    }

    private State CheckBehaviourState(float sqrMagnitude)
    {
        if (sqrMagnitude < 900)
        {
            return State.AttackState;
        }
        else if (sqrMagnitude >= 900 && sqrMagnitude < 1600)
        {
            return State.AttackMoveState;
        }
        else 
        {
            return State.MoveState;
        }
    }
}
