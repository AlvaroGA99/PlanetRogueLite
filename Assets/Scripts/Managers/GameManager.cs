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
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{   
    public static Action OnReload;
    public static bool SceneEntranceReload = false;
    public Material atmosphereMaterial;
    public Material blackHoleMaterial;
    public Projectile _projPrefab;
    public PlayerController _playerT;
    public Image energy;
    public PlanetGenerator _planetGen;
    private GravityField _gravityField;
    private Coroutine _spawner;
    private Camera cam;

    [SerializeField] UniversalRendererData rendererData;
    Blit blitFeature;
    ObjectPool<Projectile> _projectilePool;
    [SerializeField] Animation _cameraAnimation;
    [SerializeField] Animation _systemAnimation;

    private float timer;
    private float spawnval;

    [SerializeField] Button _explore;
    [SerializeField] Button _starCoords;

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

    [SerializeField] Button _backFromPause;

    private List<Vector4> planetCentres;

    private InputActionMap _menuControl;
    [SerializeField] private InputActionAsset input;
    private InputAction _sceneMenu;


    void Awake()
    {
        cam = Camera.main;
        planetCentres = new List<Vector4>();
        
        blitFeature = (Blit)rendererData.rendererFeatures[0];
        _gravityField = new GravityField();
        spawnval = 20;
        _projectilePool = new ObjectPool<Projectile>(() =>
        {
            Projectile aux = Instantiate(_projPrefab);
            aux.Init(_projectilePool,_gravityField);
            return aux;
        }, proj =>
        {
            // proj.Reset();
            proj.gameObject.SetActive(true);
        }, proj =>
        {
            proj.gameObject.SetActive(false);
        }, proj =>
        {
            Destroy(proj.gameObject);
        }, false, 50, 150);


        _explore.onClick.AddListener(OnExploreAsync);
        _starCoords.onClick.AddListener(OnStarCoords);
        _cancel.onClick.AddListener(OnCancel);
        _travel.onClick.AddListener(OnTravel);
        _mainMenu.onClick.AddListener(MainMenu);
        _newExploration.onClick.AddListener(NewExploration);
        _backFromPause.onClick.AddListener(BackFromPause);
    }

    void Start()
    {   
        Energy.OnGameOver += GameOver;
        OnReload += NewExploration;
        _menuControl = input.FindActionMap("Menu");
        _sceneMenu = _menuControl.FindAction("SceneMenu");

        _sceneMenu.performed += SceneMenu;
        blitFeature.settings.materialList.Clear();
        rendererData.SetDirty();
        timer = 0;
        planetCentres = _planetGen.GetPlanetPositions();
        _playerT.SetupGravityField(_gravityField);

        if(SceneEntranceReload){
            OnExploreAsync();
        }
    }

    void LateUpdate()
    {
        planetCentres = _planetGen.GetPlanetPositions();
        if (planetCentres.Count > 0)
        {
            for (int i = 0; i < blitFeature.settings.materialList.Count; i++)
            {
                blitFeature.settings.materialList[i].SetVectorArray("_planetCentres", planetCentres);
            }
        }
    }




    private void OnExplore()
    {
        _cameraAnimation.Play();
        StartCoroutine(WaitForAnimationFinish());
        HideStartMenu();
    }
    private void OnExploreAsync()
    {
        _titleText.gameObject.SetActive(false);
        _cameraAnimation.Play();
        ShowLoadingBar();
        HideStartMenu();
        _planetGen.Load(-1);

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
        _titleText.gameObject.SetActive(false);
        ShowLoadingBar();
        HideCoordsMenu();
        if(_starCoordsInput.text == ""){
            _planetGen.Load(-1);
        }else{
            _planetGen.Load(int.Parse(_starCoordsInput.text));
        }
        
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
    }

    private void SceneMenu(InputAction.CallbackContext action ){

        
            _fadePanel.color = new Color(_fadePanel.color.r,_fadePanel.color.g,_fadePanel.color.b,0.6f);
            _mainMenu.gameObject.SetActive(true);
            _backFromPause.gameObject.SetActive(true);
            _playerT.DisableCameraController();
            _playerT.DisablePlayerController();
            _playerT.DisableShipController();
        
      
        
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
        
        _sceneMenu.Enable();
        _playerT.camShakeIntesity = 1.0f;
        energy.enabled = true;
        blitFeature.settings.materialList.Clear();
        for (int i = 0; i < planetCentres.Count; i++)
        {
            blitFeature.settings.materialList.Add(Material.Instantiate(atmosphereMaterial));
        }
        blitFeature.settings.blitMaterial1 = Material.Instantiate(atmosphereMaterial);
        blitFeature.settings.blitMaterial.SetVector("_dirToSun", (_planetGen.transform.position));
        blitFeature.settings.blitMaterial1.SetVector("_dirToSun", (_planetGen.transform.position));
        for (int i = 0; i < blitFeature.settings.materialList.Count; i++)
        {
            blitFeature.settings.materialList[i].SetInteger("_index", i);
            
        }
        blitFeature.settings.blitMaterial1.SetInteger("_index", 0);
        blitFeature.settings.blitMaterial.SetInteger("_index", 1);
        planetCentres = _planetGen.GetPlanetPositions();

        _gravityField.AddGravityBody(new GravityBody(_planetGen.transform,300000));
        _gravityField.AddGravityBody(new GravityBody(_planetGen.blackHoleInstance.transform,100000));
        for (int i = 0; i < _planetGen._orbits.Length; i++)
        {   

            _gravityField.AddGravityBody(new GravityBody(_planetGen._orbits[i].transform, _planetGen._orbits[i].mass));
            
            _planetGen._orbits[i].SetColliders();
            planetCentres[i] = _planetGen._orbits[i].transform.position;
            _planetGen._orbits[i].transform.parent = null;

            for(int j = 0; j <20; j++){
                float theta = UnityEngine.Random.value*math.PI*2;
                float phi = UnityEngine.Random.value*math.PI;
                Vector3 direction = new Vector3(math.sin(theta)*math.cos(phi),math.sin(theta)*math.sin(phi),math.cos(theta));
                

                RaycastHit hit;
                if (Physics.Raycast(_planetGen._orbits[i].transform.position + direction*(60*_planetGen._orbits[i].transform.localScale.x/44), -direction, out hit))
                {
                    _projectilePool.Get().transform.position = hit.point ;
                }

                
            }
        }
        rendererData.SetDirty();
        _playerT.EnableShipController();
        _playerT.EnableCameraController();
    }


    private IEnumerator WaitForAnimationFinish()
    {
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

    private void GameOver()
    {   
        
        _sceneMenu.Disable();
        _playerT.DisableCameraController();
        _playerT.DisablePlayerController();
        _playerT.DisableShipController();
        _playerT.UnbindCallbacks();
        _backFromPause.gameObject.SetActive(false);
        _titleText.gameObject.SetActive(true);
        _titleText.text = "Misión Fallida";
        _fadePanel.color = new Color(_fadePanel.color.r,_fadePanel.color.g,_fadePanel.color.b,0.6f);
        _mainMenu.gameObject.SetActive(true);
        _newExploration.gameObject.SetActive(true);
    }

    private void NewExploration(){
         _playerT.DisableCameraController();
        _playerT.DisablePlayerController();
        _playerT.DisableShipController();
        _sceneMenu.Disable();
        OnReload -= NewExploration;
        Energy.OnGameOver -= GameOver;
        _sceneMenu.performed -= SceneMenu;
        _playerT.UnbindCallbacks();
        SceneEntranceReload = true;
        SceneManager.LoadScene(0);
    }


    private void BackFromPause(){
        _fadePanel.color = new Color(_fadePanel.color.r,_fadePanel.color.g,_fadePanel.color.b,0.0f);
            _newExploration.gameObject.SetActive(false);
            _mainMenu.gameObject.SetActive(false);
            _backFromPause.gameObject.SetActive(false);
            _playerT.EnableCameraController();
            _playerT.EnablePlayerController();
            _playerT.EnableShipController();
    }

    private void MainMenu(){
        Energy.OnGameOver -= GameOver;
        _sceneMenu.performed -= SceneMenu;
        _sceneMenu.Disable();
        _playerT.UnbindCallbacks();
        SceneEntranceReload = false;
        SceneManager.LoadScene(0);
    }
    private void StopSpawning()
    {
        StopCoroutine(_spawner);
    }


}
