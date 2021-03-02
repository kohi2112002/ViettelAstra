using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    private float smoothSpeed = 0.2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -1);
    private bool follow = false;
    public bool Follow
    {
        get { return follow; }
        set { follow = value; }
    }
    private void Update()
    {
        if (follow)
            transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed);
    }
}
