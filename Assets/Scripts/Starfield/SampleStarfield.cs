using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
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
    public int samples;
    private Vector3[] _dirs;
    private Vector3[] _moveDirs;
    private float[] _scales;
    private Vector3 _sampleAdjustment;
    private int _currentDirsIndex;
    private Matrix4x4[] _matrices;
    private float[] offset;
    [SerializeField] private SampleType sampleType;

    void Start()
    {
        //_starMaterial.renderQueue = 1000;
        //samples = 1000;
        float x;
        float y;
        float z;
        float theta;
        float phi;
        List<Vector3> a = new List<Vector3>();
        if (sampleType == SampleType.Conical)
        {
            _dirs = new Vector3[samples];
            _moveDirs = new Vector3[samples];
            _scales = new float[samples];
            _matrices = new Matrix4x4[samples];
            offset = new float[10];
            for (int j = 0; j < offset.Length; j++)
            {
                offset[j] = offset[j] + j * 10 / offset.Length + j * 10 / offset.Length * offset.Length - 1;
            }
        }
        else
        {
            offset = new float[1];
            offset[0] = 0;
        }


        for (int i = 0; i < samples; i++)
        {
            x = UnityEngine.Random.value;
            y = UnityEngine.Random.value;
            z = UnityEngine.Random.value;

            theta = x * math.PI * 2;
            phi = y * math.PI;
            if (sampleType == SampleType.Conical)
            {   
                _scales[i] = 0.5f;
                _sampleAdjustment = transform.forward * 20000 + transform.up * 2500;
                _dirs[i] = new Vector3(math.cos(theta) * (1 - y + 0.0001f), math.sin(theta) * (1 - y + 0.0001f), y * 10);
                _moveDirs[i] = (_dirs[i] - new Vector3(math.cos(theta) * (0.0001f), math.sin(theta) * (0.0001f), 10)).normalized;
                _dirs[i] *= 1000;
            }
            else
            {   Vector3 samp = new Vector3(2*x - 1, 2*y -1, 2*z-1);
                if(samp.magnitude < 1 && samp.magnitude > 0.6 ){
                    a.Add(samp*35000);
                }
                
                
            }

        }
        if (sampleType == SampleType.Spherical)
        {
            _dirs = new Vector3[a.Count];
            _moveDirs = new Vector3[a.Count];
            _scales = new float[a.Count];
            _matrices = new Matrix4x4[a.Count];

            for (int i = 0; i < a.Count; i++)
            {   
                _scales[i] = UnityEngine.Random.value;
                _dirs[i] = a[i];
                _moveDirs[i] = Vector3.zero;
            }

        for (int j = 0; j < offset.Length; j++)
        {
            offset[j] += Time.deltaTime;
            while (offset[j] > 10f)
            {
                offset[j] = offset[j] - 10f;
            }
            for (int i = j * _matrices.Length / offset.Length; i < (j + 1) * _matrices.Length / offset.Length; i++)
            {
                _matrices[i].SetTRS(transform.position - transform.forward * 100 + _dirs[i] + _moveDirs[i] * offset[j] + _sampleAdjustment, Quaternion.identity, new Vector3(1, 1, 1) * 150*math.clamp(_scales[i],0.4f,0.5f));
            }
        }
        }
        
        _currentDirsIndex = 0;
        _rp = new RenderParams(_starMaterial);

    }
    // Update is called once per frame
    void Update()
    {
        if (sampleType == SampleType.Conical){
        
        for (int j = 0; j < offset.Length; j++)
        {
            offset[j] += Time.deltaTime;
            while (offset[j] > 10f)
            {
                offset[j] = offset[j] - 10f;
            }
            for (int i = j * _matrices.Length / offset.Length; i < (j + 1) * _matrices.Length / offset.Length; i++)
            {
                _matrices[i].SetTRS(transform.position - transform.forward * speed * 100 + _dirs[i] + _moveDirs[i] * offset[j] * 10000 + _sampleAdjustment, Quaternion.identity, new Vector3(1, 1, 1 + speed) * 150*math.clamp(_scales[i],0.25f,0.4f));
            }
        }
        }
        
        int first = 0;
        int batches = _matrices.Length/1023;
        
        for(int i = 0; i< batches;i++){
            Graphics.RenderMeshInstanced(_rp, _starMesh, 0, _matrices,1023 ,first);
            first += 1023;
        }
        Graphics.RenderMeshInstanced(_rp, _starMesh, 0, _matrices,_matrices.Length-first, first);
        
    }

    public enum SampleType
    {
        Spherical,
        Conical
    }

}
