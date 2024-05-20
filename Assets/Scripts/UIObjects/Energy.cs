using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    public float energy;
    public static event Action OnGameOver;
    // Start is called before the first frame update
    void Start()
    {
        energy = 10;
    }

    // Update is called once per frame
    public void UpdateEnergy(float input)
    {
        energy -= input;
        if(energy < 0){
            energy = 0;
            OnGameOver?.Invoke();
            OnGameOver = null;
        }
        transform.localScale = new Vector3(energy/10,1,1);
    }

    public void SetToZero(){
        energy = 0;
        OnGameOver?.Invoke();
        OnGameOver = null;
        transform.localScale = new Vector3(0,1,1);
    }
}
