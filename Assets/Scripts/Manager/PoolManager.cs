using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PoolManager : MonoBehaviourPun
{
    public static PoolManager Instance;

    [SerializeField] private GameObject hitParticlePrefab;
    [SerializeField] private int initialPoolSize = 10;

    private Queue<GameObject> hitParticlePool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = PhotonNetwork.Instantiate(hitParticlePrefab.name, Vector3.zero, Quaternion.identity);
            obj.SetActive(false);
            hitParticlePool.Enqueue(obj);
        }
    }

    public GameObject GetHitParticle(Vector3 position, Quaternion rotation)
    {
        GameObject obj;
        if (hitParticlePool.Count > 0)
        {
            obj = hitParticlePool.Dequeue();
        }
        else
        {
            obj = PhotonNetwork.Instantiate(hitParticlePrefab.name, position, rotation);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }

    public void ReturnHitParticle(GameObject obj)
    {
        obj.SetActive(false);
        hitParticlePool.Enqueue(obj);
    }
}
