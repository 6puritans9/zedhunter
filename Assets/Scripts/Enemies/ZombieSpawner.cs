using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;  // 좀비 프리팹
    public Transform[] spawnPoints;  // 스폰 포인트
    public float spawnInterval = 3.0f;  // 좀비 스폰 간격
    public ZombieManager zombieManager;  // 좀비 매니저

    private void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned.");
            return;
        }
        StartCoroutine(SpawnZombies());
    }

    private IEnumerator SpawnZombies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnZombie();
        }
    }

    private void SpawnZombie()
    {
        if (spawnPoints.Length == 0) return;

        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];

        GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
        if (zombieManager != null)
        {
            zombieManager.AddZombie(zombie);
        }
    }
}
