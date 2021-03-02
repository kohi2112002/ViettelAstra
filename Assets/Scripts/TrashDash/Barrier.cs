using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Barrier : Obstacle
{
    private void OnEnable()
    {
        _animation = GetComponentInChildren<Animation>();
    }
    public override GameObject[] Spawn(Vector3 position, Transform parent)
    {
        GameObject go = Instantiate(gameObject, position, Quaternion.identity, parent);
        return new GameObject[] { go };
    }
    public override void PlayCollisionAnim()
    {
        _animation.Play();
        Destroy(gameObject,1.5f);
    }
}
