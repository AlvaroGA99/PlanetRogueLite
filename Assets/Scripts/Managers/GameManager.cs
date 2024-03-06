using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using Random = UnityEngine.Random;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    public static bool SceneEntranceReload = false;
    public Material atmosphereMaterial;
    public Material blackHoleMaterial;
    public Projectile _projPrefab;
    public PlayerController _playerT;
    public EnemyController _enemyPrefab;

    public Image energy;

    public PlanetGenerator _planetGen;

    private GravityField _gravityField;

    private Coroutine _spawner;
    private Camera cam;

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
    [SerializeField] RectTransform loadingBar;
    [SerializeField] RectTransform loadingBg;

    [SerializeField] Image _fadePanel;
    [SerializeField] Button _mainMenu;
    [SerializeField] Button _newExploration;
    [SerializeField] TextMeshProUGUI _titleText;

    private List<Vector4> planetCentres;

    void Awake()
    {
        cam = Camera.main;
        planetCentres = new List<Vector4>();
        _playerT.OnEnterAtmosphere += StartSpawning;
        _playerT.OnExitAtmosphere += StopSpawning;
        _playerT.OnGameOver += GameOver;
        blitFeature = (Blit)rendererData.rendererFeatures[0];
        _gravityField = new GravityField();
        _wallMask = LayerMask.GetMask("DestructionMesh");
        spawnval = 20;
        _projectilePool = new ObjectPool<Projectile>(() =>
        {
            Projectile aux = Instantiate(_projPrefab);
            aux.Init(_projectilePool,_gravityField);
            return aux;
        }, proj =>
        {
            proj.Reset();
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
            aux.Init(_projectilePool, _playerT.transform, _wallMask, _gravityField);
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

        _explore.onClick.AddListener(OnExploreAsync);
        _starCoords.onClick.AddListener(OnStarCoords);
        _options.onClick.AddListener(OnOptions);
        _cancel.onClick.AddListener(OnCancel);
        _travel.onClick.AddListener(OnTravel);
        _mainMenu.onClick.AddListener(MainMenu);
        _newExploration.onClick.AddListener(NewExploration);
    }

  

    // Start is called before the first frame update
    void Start()
    {
        blitFeature.settings.materialList.Clear();
        rendererData.SetDirty();
        timer = 0;
        planetCentres = _planetGen.GetPlanetPositions();
        // blitFeature.settings.materialList.Clear();
        // for (int i = 0; i < planetCentres.Count; i++)
        // {
        //     blitFeature.settings.materialList.Add(Material.Instantiate(atmosphereMaterial));
        // }
        // blitFeature.settings.blitMaterial1 = Material.Instantiate(atmosphereMaterial);
        // rendererData.SetDirty();
        _playerT.SetupGravityField(_gravityField);

        if(SceneEntranceReload){
            OnExploreAsync();
        }
    }

    void LateUpdate()
    {
        //blackHoleMaterial.SetMatrix("_WorldToTransformCamera",cam.transform.worldToLocalMatrix);
        planetCentres = _planetGen.GetPlanetPositions();
        print(planetCentres.Count + "PLANETAS");
        if (planetCentres.Count > 0)
        {
            // blitFeature.settings.blitMaterial.SetVectorArray("_planetCentres", planetCentres);
            // blitFeature.settings.blitMaterial1.SetVectorArray("_planetCentres", planetCentres);
            for (int i = 0; i < blitFeature.settings.materialList.Count; i++)
            {
                blitFeature.settings.materialList[i].SetVectorArray("_planetCentres", planetCentres);
            }
        }
        //planetCentres.Sort((x,y) => (new Vector4(cam.transform.position.x,cam.transform.position.y,cam.transform.position.z,0) - x).sqrMagnitude.CompareTo((new Vector4(cam.transform.position.x,cam.transform.position.y,cam.transform.position.z,0)-y).sqrMagnitude));

    }



    private IEnumerator SpawnEnemy(GameObject planet)
    {
        while (true)
        {
            // if (timer > spawnval)
            // {
            //     float xRandomOffset = Random.Range(-8, 8);
            //     float zRandomOffset = Random.Range(-8, 8);
            //     Vector3 spawnPos = _playerT.transform.position + _playerT.transform.up * 6 + _playerT.transform.forward * zRandomOffset + _playerT.transform.right * xRandomOffset;
            //     Vector3 dir = planet.transform.position - spawnPos;
            //     RaycastHit hit;
            //     if (Physics.Raycast(spawnPos, dir, out hit, _wallMask))
            //     {
            //         a = _enemyPool.Get();
            //         print("SPAWN ENEMY");
            //         a.transform.position = hit.point - dir.normalized;
            //     }
            //     spawnval = Random.Range(8, 15);
            //     timer = 0;
            // }
            // timer += Time.deltaTime;
            yield return null;
        }
    }

    private void OnExplore()
    {
        _cameraAnimation.Play();
        //_planetGen.ReloadLevel();
        StartCoroutine(WaitForAnimationFinish());
        HideStartMenu();
    }
    private void OnExploreAsync()
    {
        _titleText.gameObject.SetActive(false);
        _cameraAnimation.Play();
        ShowLoadingBar();
        HideStartMenu();
        _planetGen.Load(13);

    }

    private void OnOptions()
    {
        HideStartMenu();
    }

    private void OnStarCoords()
    {
        HideStartMenu();
        ShowCoordMenu();
    }

    private void OnTravel()
    {
        _cameraAnimation.Play();
        ShowLoadingBar();
        HideCoordsMenu();
        _planetGen.Load(int.Parse(_starCoordsInput.text));
    }

    private void OnCancel()
    {
        HideCoordsMenu();
        ShowStartMenu();
    }


    private void HideStartMenu()
    {
        _explore.gameObject.SetActive(false);
        _starCoords.gameObject.SetActive(false);
        _options.gameObject.SetActive(false);
    }

    private void HideCoordsMenu()
    {
        _starCoordsInput.gameObject.SetActive(false);
        _cancel.gameObject.SetActive(false);
        _travel.gameObject.SetActive(false);

    }

    private void ShowStartMenu()
    {
        _explore.gameObject.SetActive(true);
        _starCoords.gameObject.SetActive(true);
        _options.gameObject.SetActive(true);
    }

    private void ShowCoordMenu()
    {
        _starCoordsInput.gameObject.SetActive(true);
        _cancel.gameObject.SetActive(true);
        _travel.gameObject.SetActive(true);
    }

    private void ShowLoadingBar(){
        loadingBar.gameObject.SetActive(true);
        loadingBar.gameObject.SetActive(true);
    }

    public void StartSimulation()
    {

        _playerT.camShakeIntesity = 1.0f;
        energy.enabled = true;
        blitFeature.settings.materialList.Clear();
        for (int i = 0; i < planetCentres.Count; i++)
        {
            blitFeature.settings.materialList.Add(Material.Instantiate(atmosphereMaterial));
        }
        blitFeature.settings.blitMaterial1 = Material.Instantiate(atmosphereMaterial);
        //blitFeature.settings.blitMaterial = Material.Instantiate(atmosphereMaterial);
        blitFeature.settings.blitMaterial.SetVector("_dirToSun", (_planetGen.transform.position));
        blitFeature.settings.blitMaterial1.SetVector("_dirToSun", (_planetGen.transform.position));
        for (int i = 0; i < blitFeature.settings.materialList.Count; i++)
        {
            blitFeature.settings.materialList[i].SetInteger("_index", i);
        }
        blitFeature.settings.blitMaterial1.SetInteger("_index", 0);
        blitFeature.settings.blitMaterial.SetInteger("_index", 1);
        planetCentres = _planetGen.GetPlanetPositions();

        for (int i = 0; i < _planetGen._orbits.Length; i++)
        {
            _gravityField.AddGravityBody(new GravityBody(_planetGen._orbits[i].transform, _planetGen._orbits[i].mass));
            print(_planetGen._orbits[i].mass);
            _planetGen._orbits[i].SetColliders();
            planetCentres[i] = _planetGen._orbits[i].transform.position;
            _planetGen._orbits[i].transform.parent = null;
        }

        rendererData.SetDirty();
        _playerT.SetupHeadLook();
        _playerT.EnableShipController();
        _playerT.EnableCameraController();
        print("SIMULATION STARTED");
    }


    private IEnumerator WaitForAnimationFinish()
    {
        // while (!_planetGen.isFinishedLoading || _cameraAnimation.isPlaying)
        // {
        //     yield return null;
        // }
        _sf.gameObject.SetActive(true);
        _sfTraveling.gameObject.SetActive(false);
        _cameraAnimation.Play("SpeedDown");
        _systemAnimation.Play();
        while (_cameraAnimation.isPlaying)
        {
            yield return null;
        }
        StartSimulation();
        yield return null;
    }
    public void Arrive()
    {
        StartCoroutine(WaitForAnimationFinish());
    }
    private void StartSpawning(GameObject planet)
    {
        timer = 0;
        _spawner = StartCoroutine(SpawnEnemy(planet));
    }
    private void GameOver()
    {
        _titleText.gameObject.SetActive(true);
        _titleText.text = "Misión Fallida";
        _fadePanel.color = new Color(_fadePanel.color.r,_fadePanel.color.g,_fadePanel.color.b,0.6f);
        _mainMenu.gameObject.SetActive(true);
        _newExploration.gameObject.SetActive(true);
    }

    private void NewExploration(){
        SceneEntranceReload = true;
        SceneManager.LoadScene(0);
    }

    private void MainMenu(){
        SceneEntranceReload = false;
        SceneManager.LoadScene(0);
    }
    private void StopSpawning()
    {
        StopCoroutine(_spawner);
    }
}
