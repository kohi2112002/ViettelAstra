using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SimpleObstacle : Obstacle
{
    public void OnEnable()
    {
        _animation = GetComponentInChildren<Animation>();
    }
    public override GameObject[] Spawn(Vector3 position, Transform parent)
    {
        int numberOfObstacles = Random.Range(1, 3);
        List<int> lanes = new List<int>();
        for (int i = -1; i < 2; i++)
            lanes.Add(i);
        int spawnLane;
        List<GameObject> obstacles = new List<GameObject>();
        for (int i = 0; i < numberOfObstacles; i++)
        {
            spawnLane = lanes[Random.Range(0, lanes.Count)];
            lanes.Remove(spawnLane);
            GameObject go = Instantiate(gameObject, position + transform.right * spawnLane * TrackSegment.LANE_OFFSET, Quaternion.identity, parent);
            obstacles.Add(go);
        }
        return obstacles.ToArray();
    }
    public override void PlayCollisionAnim()
    {
        _animation.Play();
        Destroy(gameObject,1.5f);
    }
}