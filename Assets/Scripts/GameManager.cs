using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{   
    public Projectile _projPrefab;
    
    public Transform _playerT;
    public EnemyController _enemyPrefab;

    public PlanetGenerator _planetGen;

    ObjectPool<Projectile> _projectilePool;

    ObjectPool<EnemyController> _enemyPool;

    private float timer;

    EnemyController a;

    void Awake(){
         _projectilePool = new ObjectPool<Projectile>(() => {
            Projectile aux = Instantiate(_projPrefab);
            aux.Init(_playerT, _projectilePool,_planetGen.GetPlanetTransform());
            return aux ;
        }, proj => {
            proj.Reset(_playerT);
            proj.gameObject.SetActive(true);
        }, proj => {
            proj.gameObject.SetActive(false);
        }, proj => {
            Destroy(proj.gameObject);
        }, false, 50, 150);


        _enemyPool = new ObjectPool<EnemyController>(() => {

            EnemyController aux = Instantiate(_enemyPrefab);
            aux.Init(_projectilePool,_playerT,_planetGen.GetPlanetTransform());
            return aux ;
        }, enemy => {
            enemy.gameObject.SetActive(true);
        }, enemy => {
            enemy.gameObject.SetActive(false);
        }, enemy => {
            Destroy(enemy.gameObject);
        }, false, 5, 15);
    }
    // Start is called before the first frame update
    void Start()
    {  
       timer = 0;
       a = _enemyPool.Get();
       a.transform.position = new Vector3(.4711895f,54.53f,6.3f); 
    }

    // Update is called once per frame
    void Update()
    {   
        if(timer > 8){
            SpawnEnemy();
        }
        timer += Time.deltaTime;
    }

    private void SpawnEnemy(){
        
    }
}
