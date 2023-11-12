using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class DestructionMesh : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 _displacement;
    private Rigidbody _rb;
    private Vector3 _planetPosition;
    private MeshFilter _mF;
    private Mesh _m;
    void Awake(){
       
        _rb = GetComponent<Rigidbody>();
        _mF = gameObject.AddComponent<MeshFilter>();
        //_mF.mesh = _m;
        _planetPosition = transform.position;
    }

    void Start(){
         transform.position += _displacement*44;
         _rb.AddExplosionForce(5,_planetPosition,_displacement.magnitude);
         StartCoroutine("DestroyFade");
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Vector3 displacement,Mesh destructionMesh){
        _displacement = displacement;
        _m = destructionMesh;
        _mF.mesh = _m;
    }

    IEnumerator DestroyFade(){
        float timer = 0;
        while(timer < 3){
            timer += Time.deltaTime;
            this.gameObject.transform.localScale -= new Vector3(14.67f,14.67f,14.67f)*Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
