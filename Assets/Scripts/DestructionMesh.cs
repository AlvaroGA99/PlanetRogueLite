using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionMesh : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 _displacement;
    private Rigidbody _rb;
    private Vector3 _planetPosition;
    void Awake(){
       
        _rb = GetComponent<Rigidbody>();
        _planetPosition = transform.position;
    }

    void Start(){
         transform.position += _displacement;
         _rb.AddExplosionForce(5,_planetPosition,_displacement.magnitude);
         StartCoroutine("DestroyFade");
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Vector3 displacement){
        _displacement = displacement;
    }

    IEnumerator DestroyFade(){
        float timer = 0;
        while(timer < 3){
            timer += Time.deltaTime;
            this.gameObject.transform.localScale -= new Vector3(Time.deltaTime/3,Time.deltaTime/3,Time.deltaTime/3);
            yield return null;
        }
    }
}
