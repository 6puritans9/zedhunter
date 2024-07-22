using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
    {
        [Header("Player Info")]
        private GameObject _player;
        public int maxPlayerHealth = 100;
        public int currentPlayerHealth = 100;
        [HideInInspector] public bool isDead = false;

        [Header("Respawn")] private PlayerSpawner _playerSpawner;

        [Header("Player Health")] private UIManager _uiManager;

        // Start is called before the first frame update
        void Start()
            {
                _player = GameObject.FindWithTag("Player");
                _playerSpawner = GameObject.FindObjectOfType<PlayerSpawner>();
                _uiManager = FindObjectOfType<UIManager>();
            }

        // Update is called once per frame
        void Update()
            {
                _uiManager.UpdateHealthEffect(currentPlayerHealth, maxPlayerHealth);
            }

        public void TakeDamage(int damage)
            {
                Debug.Log($"ActionStateManager: TakeDamage called with damage: {damage}");
                if (currentPlayerHealth > 0)
                    {
                        currentPlayerHealth -= damage;
                        if (currentPlayerHealth <= 0)
                            {
                                PlayerDeath();
                            }
                        else
                            Debug.Log("Player Hit!!");
                    }
            }

        void PlayerDeath()
            {
                // TODO: need fix
                _player.SetActive(false);

                isDead = true;
                StartCoroutine(DelayedRespawn(5f));
                Invoke("RespawnPlayer", 5f);
            }

        IEnumerator DelayedRespawn(float delay)
            {
                yield return new WaitForSeconds(delay);
                _playerSpawner.RespawnPlayer();
            }
    }