using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public Projectile _projPrefab;
    public PlayerController _playerT;
    public EnemyController _enemyPrefab;

    public PlanetGenerator _planetGen;

    private GravityField _gravityField;

    [SerializeField] UniversalRendererData rendererData;
    Blit blitFeature;
    ObjectPool<Projectile> _projectilePool;

    ObjectPool<EnemyController> _enemyPool;

    [SerializeField] Animation _cameraAnimation;
    [SerializeField] Animation _systemAnimation;

    private float timer;
    private float spawnval;
    private LayerMask _wallMask;
    EnemyController a;

    [SerializeField] Button _explore;
    [SerializeField] Button _starCoords;
    [SerializeField] Button _options;

    [SerializeField] InputField _starCoordsInput;

    [SerializeField] Button _travel;

    [SerializeField] Button _cancel;

    [SerializeField] private SampleStarfield _sf;
    [SerializeField] private SampleStarfield _sfTraveling;

    void Awake()
    {   
        blitFeature = (Blit)rendererData.rendererFeatures[0];
        _gravityField = new GravityField();
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

        _explore.onClick.AddListener(OnExplore);
        _starCoords.onClick.AddListener(OnStarCoords);
        _options.onClick.AddListener(OnOptions);
        _cancel.onClick.AddListener(OnCancel);
    }
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        _playerT.SetupGravityField(_gravityField);
    }

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

    private void OnExplore(){
        _cameraAnimation.Play();
        //_planetGen.ReloadLevel();
        StartCoroutine(WaitForLevelAndAnimationFinish());
        HideStartMenu();
    }

    private void OnOptions(){
        HideStartMenu();
    }

    private void OnStarCoords(){
        HideStartMenu();
        ShowCoordMenu();
    }

    private void OnTravel(){

    }

    private void OnCancel(){
        HideCoordsMenu();
        ShowStartMenu();
    }


    private void HideStartMenu(){
        _explore.gameObject.SetActive(false);
        _starCoords.gameObject.SetActive(false);
        _options.gameObject.SetActive(false);
    }

    private void HideCoordsMenu(){
        _starCoordsInput.gameObject.SetActive(false);
        _cancel.gameObject.SetActive(false);
        _travel.gameObject.SetActive(false);
        
    }

    private void ShowStartMenu(){
        _explore.gameObject.SetActive(true);
        _starCoords.gameObject.SetActive(true);
        _options.gameObject.SetActive(true);
    }

    private void ShowCoordMenu(){
        _starCoordsInput.gameObject.SetActive(true);
        _cancel.gameObject.SetActive(true);
        _travel.gameObject.SetActive(true);
    }

    private void StartSimulation(){
        foreach(Planet p in _planetGen._orbits){
            _gravityField.AddGravityBody(new GravityBody(p.transform,p.mass));
            print(p.mass);
            p.SetColliders();
            //blitFeature.settings.blitMaterial.SetVector("_WorldSpherePos",p.transform.position);
            p.transform.parent = null;
        }
        //rendererData.SetDirty();
        _playerT.EnableShipController();
        _playerT.EnableCameraController();
        print("SIMULATION STARTED");
    }


    private IEnumerator WaitForLevelAndAnimationFinish(){
        while(!_planetGen.isFinishedLoading || _cameraAnimation.isPlaying){
            yield return null;
        }
        _sf.gameObject.SetActive(true);
        _sfTraveling.gameObject.SetActive(false);
        _cameraAnimation.Play("SpeedDown");
        _systemAnimation.Play();
        while(_cameraAnimation.isPlaying){
            yield return null;
        }
        StartSimulation();
        yield return null;
    }
}
