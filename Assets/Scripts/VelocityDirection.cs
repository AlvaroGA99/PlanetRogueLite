using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VelocityDirection : MonoBehaviour
{
    [SerializeField] Rigidbody _ship;
    [SerializeField] Material _mat;
    [SerializeField] TextMeshProUGUI _velocityMagnitude;
    [SerializeField] Transform _camTransform;

    public float velocityMag;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        float mag = _ship.velocity.magnitude;
        velocityMag = Mathf.Clamp01(mag/100);
        Color c = new Color(0,1,0,velocityMag);
        _velocityMagnitude.color = c;
        _mat.SetColor("_Color",c);
        _velocityMagnitude.text = (int)mag + " kms"; 
        // _mat.SetColor("_Color",new Color(1,1,1,0));
        transform.rotation = Quaternion.LookRotation(_camTransform.worldToLocalMatrix*_ship.velocity);
    }
}
