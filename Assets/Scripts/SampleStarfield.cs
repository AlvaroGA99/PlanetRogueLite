using System.Collections;
using System.Collections.Generic;
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
    private Vector3[] _dirs;
    private float[] _scales;

    Matrix4x4[] matrices;
    void Start()
    {   
        //_starMaterial.renderQueue = 1000;
        _samples = 1000;
        _dirs = new Vector3[_samples];
        _scales = new float[_samples];

        matrices = new Matrix4x4[_samples];
        for (int i = 0; i < _samples; i++)
        {
            float x =  UnityEngine.Random.value;
            float y = UnityEngine.Random.value;

            _scales[i] = UnityEngine.Random.value;

            float theta = x*math.PI*2;
            float phi = y*math.PI;

            _dirs[i] = new Vector3(math.sin(phi)*math.cos(theta),math.sin(phi)*math.sin(theta),math.cos(phi));
            //matrices[i] = Matrix4x4.Translate(transform.position + _dirs[i]*5);
            matrices[i].SetTRS(transform.position + _dirs[i]*5,Quaternion.identity,Vector3.one/10f*_scales[i]);
        }
        _rp = new RenderParams(_starMaterial);
        
        
    }

    // Update is called once per frame
    void Update()
    { 
            for (int i = 0; i < _samples; i++)
         {
            matrices[i].SetTRS(transform.position - transform.forward*speed*100 + _dirs[i]*15000,Quaternion.identity,new Vector3(1,1,1+speed)*100);  
         }
        Graphics.RenderMeshInstanced(_rp,_starMesh,0,matrices);
        
    }
}
