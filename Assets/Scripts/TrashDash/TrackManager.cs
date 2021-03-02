using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TrackManager : MonoBehaviour
{
    public static TrackManager thisS;
    [ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)] public Color32 flickCol1, flickCol2;
    private int totalConcurrentSegment = 3;
    [SerializeField] private TrackSegment[] trackPrefabs;
    [SerializeField] private GameObject coinPrefab, coinPrefab2, coinPrefab3;
    private List<GameObject> tracksPool;
    public List<GameObject> activeTracks;

    private void Awake()
    {
        thisS = GetComponent<TrackManager>();
    }

    protected void Start()
    {
        tracksPool = new List<GameObject>();
        StartCoroutine(SpawnSegmentLoop());
    }
    private IEnumerator SpawnSegmentLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.5f);
            if (activeTracks.Count < totalConcurrentSegment) SpawnNewSegment();
        }

    }
    public void SpawnNewSegment()
    {
        GameObject trackSegment;
        if (tracksPool.Count > 0)
        {
            trackSegment = tracksPool[Random.Range(0, tracksPool.Count)];
            trackSegment.transform.position = GetSegmentSpawnPos();
            trackSegment.SetActive(true);
            tracksPool.Remove(trackSegment);
        }
        else
            trackSegment = Instantiate(trackPrefabs[Random.Range(0, trackPrefabs.Length)].gameObject, GetSegmentSpawnPos(), Quaternion.identity);
        activeTracks.Add(trackSegment);
    }
    private Vector3 GetSegmentSpawnPos()
    {
        Vector3 spawnPos = Vector3.zero;
        if (activeTracks.Count != 0)
        {
            GameObject go = activeTracks[activeTracks.Count - 1];
            spawnPos = go.transform.position + new Vector3(0, 0, go.GetComponent<TrackSegment>().deltaToNextSegment);
        }
        return spawnPos;
    }
    public void ReturnSegmentToPool(GameObject segment)
    {
        if (activeTracks.Contains(segment))
        {
            segment.SetActive(false);
            activeTracks.Remove(segment);
            tracksPool.Add(segment);
        }
    }
    public GameObject SpawnCoin(Vector3 position)
    {
        float random = Random.Range(0, 1f);
        return Instantiate(random < 0.9f ? coinPrefab : (Random.Range(0, 1f) < 0.9f ? coinPrefab2 : coinPrefab3), position, Quaternion.identity) as GameObject;
    }
}