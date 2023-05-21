using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject[] fishPrefabs;
    public Vector3 minBounds;
    public Vector3 maxBounds;
    public int fishCount = 10;

    void Start()
    {
        Vector3 center = transform.position;
        Vector3 halfSize = (maxBounds - minBounds) / 2f;
        Vector3 newMinBounds = center - halfSize;
        Vector3 newMaxBounds = center + halfSize;
        for (int i = 0; i < fishCount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(newMinBounds.x, newMaxBounds.x),
                Random.Range(newMinBounds.y, newMaxBounds.y),
                Random.Range(newMinBounds.z, newMaxBounds.z)
            );
            GameObject fishPrefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];
            GameObject fish = Instantiate(fishPrefab, position, Quaternion.identity);
            Fish fishScript = fish.GetComponent<Fish>();
            fishScript.minBounds = newMinBounds;
            fishScript.maxBounds = newMaxBounds;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position;
        Vector3 size = maxBounds - minBounds;
        Gizmos.DrawWireCube(center, size);
    }
}
