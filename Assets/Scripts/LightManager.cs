using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{   
    [SerializeField] private Transform _mainTarget;
    [SerializeField] private Transform _shipTarget;
    [SerializeField] private Light _mainLight;
    [SerializeField] private Light _shipLight;
    private List<Light> _lights;
    private List<Transform> _targets;
    private int _layer;
    // Start is called before the first frame update

    void Awake()
    {   
        _targets = new List<Transform>();
        _lights = new List<Light>();
        _layer = 9;
    }

    // Update is called once per frame
    void Update()
    {   
        _mainLight.transform.LookAt(_mainTarget);
        _shipLight.transform.LookAt(_shipTarget);
        for (int i = 0; i < _lights.Count; i++)
        {
             _lights[i].transform.LookAt(_targets[i]);
        }
       
    }

    public void AddLight(Transform planetTarget){
        Light a = Instantiate(_mainLight,transform);
        a.gameObject.layer = _layer;
        planetTarget.gameObject.layer = _layer;
        _layer ++;
        _lights.Add(a);
        _targets.Add(planetTarget);
    }

    public void Clear(){
        _targets.Clear();
        for (int i = 0; i < _lights.Count; i++)
        {
            Destroy(_lights[i].gameObject);
        }
        _lights.Clear();
        _layer = 9;
    }
}
