using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CenterAlignTutorial : MonoBehaviour
{
    [SerializeField] private int thresholdTime = 3;
    private float totalTime;
    public bool IsPass()
    {
        return totalTime > thresholdTime;
    }
    private Renderer meshRenderer;
    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        meshRenderer.material.color = Color.green;
    }
    private void OnTriggerStay(Collider other)
    {
        totalTime += Time.deltaTime;
    }
    private void OnTriggerExit(Collider other)
    {
        totalTime = 0;
        meshRenderer.material.color = Color.red;
    }
}
