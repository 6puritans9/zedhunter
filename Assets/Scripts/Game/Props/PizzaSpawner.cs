using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PizzaSpawner : MonoBehaviour
    {
        public GameObject pizza;
        public Transform[] spawnPoints;
        
        private bool[] hasPizza = new bool[9];
        private bool isSpawning = false;
        
    void Start()
    {
        for (int i = 0; i < hasPizza.Length; i += 1)
            {
                hasPizza[i] = false;
            }
    }

    // Update is called once per frame
    void Update()
        {
            if(!isSpawning)
                StartCoroutine(SpawnPizza());
        }

    private IEnumerator SpawnPizza()
        {
            isSpawning = true;
            
            for (int i = 0; i < spawnPoints.Length; i += 1)
                {
                    if (!hasPizza[i])
                        {
                            Instantiate(pizza, spawnPoints[i].position, spawnPoints[i].rotation);
                            hasPizza[i] = true;

                            yield return new WaitForSeconds(5f);
                        }
                }

            isSpawning = false;
        }
}
