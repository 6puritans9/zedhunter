// using System.Collections;
// using System.Collections.Generic;
// using Unity.Mathematics;
// using UnityEngine;
//
// public class PizzaSpawner : MonoBehaviour
//     {
//         public GameObject pizza;
//         public Transform[] spawnPoints;
//
//         private bool[] hasPizza = new bool[9];
//         private bool isSpawning = false;
//         public int MAX_PIZZA_COUNT = 3;
//         private int currentPizzaCount = 0;
//
//         void Start()
//             {
//                 for (int i = 0; i < hasPizza.Length; i += 1)
//                     {
//                         hasPizza[i] = false;
//                     }
//             }
//
//         // Update is called once per frame
//         void Update()
//             {
//                 if (!isSpawning && currentPizzaCount < MAX_PIZZA_COUNT)
//                     {
//                         int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
//                         StartCoroutine(SpawnPizza(randomIndex));
//                     }
//             }
//
//         private IEnumerator SpawnPizza(int index)
//             {
//                 isSpawning = true;
//
//                 if (!hasPizza[index])
//                     {
//                         Instantiate(pizza, spawnPoints[index].position, spawnPoints[index].rotation);
//                         hasPizza[index] = true;
//
//                         yield return new WaitForSeconds(15f);
//                     }
//
//                 isSpawning = false;
//             }
//     }

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PizzaSpawner : MonoBehaviour
    {
        public PizzaManager pizzaManager;
        
        public GameObject pizzaPrefab;
        public Transform[] spawnPoints;

        private bool[] hasPizza = new bool[9];
        private bool isSpawning = false;
        public int MAX_PIZZA_COUNT = 3;
        private int currentPizzaCount = 0;

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
                if (!isSpawning && currentPizzaCount < MAX_PIZZA_COUNT)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
                        StartCoroutine(SpawnPizza(randomIndex));
                    }
            }

        private IEnumerator SpawnPizza(int index)
            {
                isSpawning = true;

                if (!hasPizza[index])
                    {
                        GameObject newPizza = Instantiate(pizzaPrefab, spawnPoints[index].position, spawnPoints[index].rotation);
                        hasPizza[index] = true;

                        yield return new WaitForSeconds(15f);
                    }

                isSpawning = false;
            }
    }