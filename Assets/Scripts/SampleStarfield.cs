using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SampleStarfield : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform _camTransform;
    [SerializeField] private Material _starMaterial;
    [SerializeField] private Mesh _starMesh;
    private RenderParams _rp;
    public int speed;
    private int _samples;
    private int _frames;
    private int _chunks;
    private Vector3[] _dirs;
    private Vector3[] _originalDirs;
    private Vector3[] _moveDirs;
    private float[] _scales;
    private int _currentDirsIndex;

    IEnumerator cor;
    Matrix4x4[] matrices;
   [SerializeField] public float[] offset;

    void Start()
    {
        //_starMaterial.renderQueue = 1000;
        _chunks = 3;
        _samples = 1000;
        _frames = 7;
        _dirs = new Vector3[_samples];
        _moveDirs = new Vector3[_samples];
        _scales = new float[_samples];
        matrices = new Matrix4x4[_samples];
        float x;
        float y;
        float theta;
        //float phi;
         for (int j = 0; j < offset.Length; j++)
        {
            offset[j] = offset[j] + j*10/offset.Length + j*10/offset.Length*offset.Length-1;
        }
        for (int i = 0; i < _samples; i++)
        {
            //     for (int i = j*_samples/_chunks; i < (j+1)*_samples/_chunks; i++)
            // {   
            _scales[i] = UnityEngine.Random.value;


            x = UnityEngine.Random.value;
            y = UnityEngine.Random.value;



            theta = x * math.PI * 2;
            //phi = y*math.PI;

            _dirs[i] = new Vector3(math.cos(theta) * (1 - y + 0.0001f), math.sin(theta) * (1 - y + 0.0001f), y*10);
            //_dirs[i] = new Vector3(math.sin(phi)*math.cos(theta),math.sin(phi)*math.sin(theta), math.cos(phi)  );
            _moveDirs[i] = (_dirs[i] - new Vector3(math.cos(theta) * (0.0001f), math.sin(theta) * (0.0001f), 10)).normalized;

            //matrices[i].SetTRS(transform.position + _dirs[i]*5,Quaternion.identity,Vector3.one/10f*_scales[i]);
            //}   
        }
        _currentDirsIndex = 0;
        _rp = new RenderParams(_starMaterial);

        // cor = SwappingStars();
        // StartCoroutine(cor);

    }

    private void NextDirs()
    {
        if (_currentDirsIndex < offset.Length - 1)
        {
            _currentDirsIndex++;
        }
        else
        {
            _currentDirsIndex = 0;
        }

        for (int i = _currentDirsIndex * _samples / offset.Length; i < (_currentDirsIndex + 1) * _samples / _chunks; i++)
        {
            //_dirs[i] -= _moveDirs[i] ;

            //matrices[i].SetTRS(transform.position + _dirs[i]*5,Quaternion.identity,Vector3.one/10f*_scales[i]);
        }

    }
    // Update is called once per frame
    void Update()
    {
        for (int j = 0; j < offset.Length; j++)
        {
            offset[j] += Time.deltaTime;
            while(offset[j] > 10f){
                offset[j] = offset[j] - 10f;
            }
            for (int i = j * _samples / offset.Length; i < (j + 1) * _samples / offset.Length; i++)
            {
                matrices[i].SetTRS(transform.position - transform.forward * speed * 100 + _dirs[i] * 1000 + _moveDirs[i] * offset[j] * 10000 + transform.forward*20000 + transform.up*2500, Quaternion.identity, new Vector3(1, 1, 1 + speed)*100);
            }
        }
        print(_currentDirsIndex);
        Graphics.RenderMeshInstanced(_rp, _starMesh, 0, matrices);

    }

}
