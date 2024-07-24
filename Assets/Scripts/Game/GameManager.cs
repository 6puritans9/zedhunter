using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [Header("Player Character Index")]
        [HideInInspector] public int avatarIndex;
        
        [Header("Player Info")]
        public int playerScore;

        private void Awake()
            {
                if (instance != null && instance != this)
                    {
                        Destroy(gameObject);
                        return;
                    }

                instance = this;
                DontDestroyOnLoad(gameObject);
            }

        public void SetPlayerAvatar(int index)
            {
                avatarIndex = index;
            }
        
        public int GetPlayerAvatar()
            {
                return avatarIndex;
            }
        
        public void InitializeGame()
            {
                playerScore = 0;
            }
    }