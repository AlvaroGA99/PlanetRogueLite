using System.Collections;
using UnityEngine;

public class DestructionMesh : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 _displacement;
    private Rigidbody _rb;
    private MeshCollider _mc;
    private SphereCollider _sc;
    private Vector3 _planetPosition;
    private MeshFilter _mF;
    private Mesh _m;
    void Awake(){
       
        _rb = GetComponent<Rigidbody>();
        _mF = gameObject.AddComponent<MeshFilter>();
        _planetPosition = transform.position;
    }

    void Start(){
         transform.position += _displacement*44;
         
         if(Random.Range(0,5) == 0){
            //Destroy(_rb);
            _rb.isKinematic = true;
            _mc = gameObject.AddComponent<MeshCollider>();
            _mc.sharedMesh = _m;
            StartCoroutine("DestroyFall");
         }else{
            _sc = gameObject.AddComponent<SphereCollider>();
            StartCoroutine("DestroyFade");
         }
         
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
        _rb.AddExplosionForce(50,_planetPosition,_displacement.magnitude);
        float timer = 0;
        while(timer < 9){
            timer += Time.deltaTime;
            //this.gameObject.transform.localScale -= new Vector3(14.67f,14.67f,14.67f)*Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
    IEnumerator DestroyFall(){
        Vector3 dir = _displacement.normalized;
        float timer = 0;
        float vel = 0;
        while(timer < 1.5){
            vel += 9.8f*Time.deltaTime;
            timer += Time.deltaTime;
            this.gameObject.transform.position -= vel*dir*Time.deltaTime;
            yield return null;
        }
        
    }
}
