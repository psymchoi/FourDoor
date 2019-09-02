using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandLightRange : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LightRange(0.15f));
    }
    
    IEnumerator LightRange(float _delayTime)
    {
        this.GetComponent<Light>().intensity = Random.Range(1, 7);
        yield return new WaitForSeconds(_delayTime);
        StartCoroutine(LightRange(Random.Range(0.15f, 0.85f)));
    }

}
