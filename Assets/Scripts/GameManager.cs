using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{   
    Projectile _projPrefab;

    Transform _playerT;
    EnemyController _enemyPrefab;

    ObjectPool<Projectile> _projectilePool;

    ObjectPool<EnemyController> _enemyPool;
    // Start is called before the first frame update
    void Start()
    {
        _projectilePool = new ObjectPool<Projectile>(() => {
            return Instantiate(_projPrefab);
        }, proj => {
            proj.gameObject.SetActive(true);
        }, proj => {
            proj.gameObject.SetActive(false);
        }, proj => {
            Destroy(proj.gameObject);
        }, false, 50, 150);


        _enemyPool = new ObjectPool<EnemyController>(() => {

            EnemyController aux = Instantiate(_enemyPrefab);
            aux.Init(_projectilePool);
            return aux ;
        }, enemy => {
            enemy.gameObject.SetActive(true);
        }, enemy => {
            enemy.gameObject.SetActive(false);
        }, enemy => {
            Destroy(enemy.gameObject);
        }, false, 5, 15);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
