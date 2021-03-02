using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// This defines a "piece" of the track. This is attached to the prefab and contains data such as what obstacles can spawn on it.
/// It also defines places on the track where obstacles can spawn. The prefab is placed into a ThemeData list.
/// </summary>
public class TrackSegment : MonoBehaviour
{
    public const float LANE_OFFSET = 1.5f;
    public float deltaToNextSegment = 18;
    [SerializeField] private int maxObstacles = 3;
    [SerializeField] private Obstacle[] obstacles;
    private Vector3[] obstaclesPos;
    [SerializeField] private List<GameObject> activeObstacles = new List<GameObject>(), activeCoins = new List<GameObject>();
    private TrackManager trackManager;
    private void OnEnable()
    {
        trackManager = FindObjectOfType<TrackManager>();
        obstaclesPos = new Vector3[maxObstacles];
        for (int i = 0; i < maxObstacles; i++)
        {
            Vector3 oSpawnPos = transform.position + new Vector3(0, 0, deltaToNextSegment * Random.Range(0.2f, 0.3f) * i);
            if (i == 0)
                oSpawnPos += new Vector3(0, 0, 1.5f);
            else
            if (i == maxObstacles - 1)
                oSpawnPos -= new Vector3(0, 0, 1.5f);
            obstaclesPos[i] = oSpawnPos;
        }
        if (transform.position.z > 50)
            SpawnObstacles();
        if (transform.position.z > 30)
            SpawnCoinAndPowerUp();
    }
    private void OnDisable()
    {
        for (int i = activeObstacles.Count - 1; i > -1; i--)
            Destroy(activeObstacles[i]);
        for (int i = activeCoins.Count - 1; i > -1; i--)
            Destroy(activeCoins[i]);
    }
    private void OnTriggerEnter(Collider other)
    {
        trackManager.ReturnSegmentToPool(gameObject);
    }
    public void SpawnObstacles()
    {
        int numberOfObstacles = Random.Range(1, maxObstacles);
        List<int> availablePos = new List<int>();
        for (int i = 0; i < maxObstacles; i++)
            availablePos.Add(i);
        int spawnPos;
        float probability = FindObjectOfType<GameController>().ObstacleSpawnProbability();
        for (int i = 0; i < numberOfObstacles; i++)
        {
            spawnPos = availablePos[Random.Range(0, availablePos.Count)];
            availablePos.Remove(spawnPos);
            activeObstacles.AddRange(obstacles[Random.Range(0, obstacles.Length)].Spawn(obstaclesPos[spawnPos], transform));
            if (availablePos.Count * 1f / maxObstacles < probability) break;
        }
    }
    public void SpawnCoinAndPowerUp()
    {
        float currentPos = 0f, worldLeng = deltaToNextSegment;
        int actualLane = Random.Range(-1, 2), testLane = actualLane + 1;
        GameObject coin;
        while (currentPos < worldLeng)
        {
            bool laneValid = true;
            while (Physics.CheckSphere(transform.position + currentPos * Vector3.forward + (testLane - 1) * LANE_OFFSET * Vector3.right + new Vector3(0, 0.5f, 0), 0.4f, LayerMask.GetMask("Obstacle")))
            {
                testLane = (testLane + 1) % 3;
                currentPos += 1;
                if (testLane == actualLane + 1)
                {
                    currentPos += 2;
                    laneValid = false;
                    break;//if we loop through all lane
                }
            }
            if (laneValid)
            {
                actualLane = testLane - 1;
                coin = trackManager.SpawnCoin(transform.position + currentPos * Vector3.forward + actualLane * LANE_OFFSET * Vector3.right + new Vector3(0, 0.5f, 0));
                activeCoins.Add(coin);
            }
            currentPos += 1f;
        }
    }
}