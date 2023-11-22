using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    public Projectile _projPrefab;
    public PlayerController _playerT;
    public EnemyController _enemyPrefab;

    public PlanetGenerator _planetGen;

    private GravityField _gravityField;

    ObjectPool<Projectile> _projectilePool;

    ObjectPool<EnemyController> _enemyPool;

    private float timer;
    private float spawnval;
    private LayerMask _wallMask;
    private Transform[] _preloadedGravityBodies;

    EnemyController a;

    void Awake()
    {   
        _gravityField = new GravityField();
        // foreach(Transform t in _preloadedGravityBodies){
        //     GravityBody g = new GravityBody(t,0.5f);
        //     _gravityField.AddGravityBody(g);
        // }
        _wallMask = LayerMask.GetMask("DestructionMesh");
        spawnval = 8;
        _projectilePool = new ObjectPool<Projectile>(() =>
        {
            Projectile aux = Instantiate(_projPrefab);
            aux.Init(_playerT.transform, _projectilePool);
            return aux;
        }, proj =>
        {
            proj.Reset(_playerT.transform);
            proj.gameObject.SetActive(true);
        }, proj =>
        {
            proj.gameObject.SetActive(false);
        }, proj =>
        {
            Destroy(proj.gameObject);
        }, false, 50, 150);


        _enemyPool = new ObjectPool<EnemyController>(() =>
        {

            EnemyController aux = Instantiate(_enemyPrefab);
            aux.Init(_projectilePool, _playerT.transform, _wallMask);
            return aux;
        }, enemy =>
        {
            enemy.gameObject.SetActive(true);
        }, enemy =>
        {
            enemy.gameObject.SetActive(false);
        }, enemy =>
        {
            Destroy(enemy.gameObject);
        }, false, 5, 15);
    }
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        a = _enemyPool.Get();
        a.transform.position = new Vector3(.4711895f, 54.53f, 6.3f);
    }

    // Update is called once per frame
    void Update()
    {
        // if (_planetGen.IsInSpawnState())
        // {
        //     if (timer > spawnval)
        //     {
        //         SpawnEnemy();

        //         spawnval = Random.Range(8, 15);
        //         timer = 0;
        //     }
        //     timer += Time.deltaTime;
        // }

    }

    // private void SpawnEnemy()
    // {   
       
    //     float xRandomOffset = Random.Range(-8, 8);
    //     float zRandomOffset = Random.Range(-8, 8);
    //     Vector3 spawnPos = _playerT.transform.position + _playerT.transform.forward*zRandomOffset + _playerT.transform.right*xRandomOffset;
    //     Vector3 dir = _planetGen.GetPlanetTransform().position - spawnPos;
    //     RaycastHit hit;

    //     // while(!Physics.Raycast(spawnPos,dir,out hit,_wallMask)){
    //     //     xRandomOffset = Random.Range(-8,8);
    //     //     zRandomOffset = Random.Range(-8,8);
    //     //     spawnPos = transform.localToWorldMatrix*new Vector3(xRandomOffset,0,zRandomOffset);
    //     // }
    //     if (Physics.Raycast(spawnPos, dir, out hit,_wallMask))
    //     {   
    //         a = _enemyPool.Get();
    //          print("SPAWN ENEMY");
    //         a.transform.position = hit.point - dir.normalized;
    //     }



    //     //samplear angulo random alrededor del personaje dentro de un radio
    //     //spawnear el enemigo.
    // }
}
