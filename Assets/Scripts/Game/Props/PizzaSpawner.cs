using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaSpawner : MonoBehaviour
    {
        public PizzaManager pizzaManager;
        
        public GameObject pizzaPrefab;
        public Transform[] spawnPoints;

        [HideInInspector] public bool[] hasPizza = new bool[8];
        private bool isSpawning = false;
        public int MAX_PIZZA_COUNT;
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
                        newPizza.GetComponent<Pizza>().spawnIndex = index;
                        hasPizza[index] = true;

                        yield return new WaitForSeconds(15f);
                    }

                isSpawning = false;
            }
        
        public void PizzaDisappeared(Pizza pizza)
            {
                print("Pizza Disappeared");
                hasPizza[pizza.spawnIndex] = false;
            }
        
        public void PizzaDestroyed(int index)
            {
                if (index >= 0 && index < hasPizza.Length)
                    {
                        hasPizza[index] = false;
                        currentPizzaCount--;
                    }
            }
    }