using UnityEngine;
using System.Collections.Generic;

public class ZombieManager : MonoBehaviour
{
    public int maxZombies = 100;  // 최대 좀비 수
    private List<GameObject> zombies = new List<GameObject>();

    public void AddZombie(GameObject zombie)
    {
        zombies.Add(zombie);
        if (zombies.Count > maxZombies)
        {
            RemoveOldestZombie();
        }
    }

    private void RemoveOldestZombie()
    {
        if (zombies.Count > 0)
        {
            GameObject oldestZombie = zombies[0];
            zombies.RemoveAt(0);
            Destroy(oldestZombie);
        }
    }
}
