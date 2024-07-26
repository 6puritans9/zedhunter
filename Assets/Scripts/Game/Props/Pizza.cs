using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pizza : MonoBehaviour
    {
        private GameManager _gameManager;
        
        public float rotationSpeed = 30f;

        public float[] timers;
        private float currentTime;
        public int spawnIndex;
        public int HP;

        private void Start()
            {
                int randomIndex = UnityEngine.Random.Range(0, timers.Length);

                _gameManager = FindObjectOfType<GameManager>();
                currentTime = timers[randomIndex];
            }

        void Update()
            {
                RotatePizza();
                CheckTimer();
            }

        private void RotatePizza()
            {
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }

        private void CheckTimer()
            {
                currentTime -= Time.deltaTime;

                if (currentTime <= 0)
                    {
                        PizzaSpawner pizzaSpawner = FindObjectOfType<PizzaSpawner>();
                        pizzaSpawner.PizzaDisappeared(this);
                        Destroy(gameObject);
                    }
            }

        public void TakeDamage(int damage)
            {
                if (HP <= 0) return;

                HP -= damage;

                if (HP <= 0)
                    {
                        StartCoroutine(DestroyWallWithEffect()); // ���� �ı��� �� ȿ�� ����
                    }
            }

        private IEnumerator DestroyWallWithEffect()
            {
                yield return null;
                // Destroy(gameObject);
                OnPizzaDestroyed();
            }

        private void OnPizzaDestroyed()
            {
                PizzaSpawner pizzaSpawner = FindObjectOfType<PizzaSpawner>();
                if (pizzaSpawner != null)
                    {
                        pizzaSpawner.PizzaDestroyed(spawnIndex);
                    }
                
                _gameManager.SubtractPizzaItemScore();
                PizzaManager.Instance?.PizzaDestroyed(this);
                Destroy(gameObject);
            }
    }