using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlick : MonoBehaviour
{
    
    const float lifeTime = 1, speed=1;
    float currentTime;
    bool currentHigh = true;
    const string hash_color = "_EmissionColor";
    Material mat;

    private void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
        currentTime = Random.Range(0f,lifeTime);
    }
}
