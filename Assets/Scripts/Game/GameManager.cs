using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [Header("Player Character Index")]
        [HideInInspector] public int avatarIndex;

        [Header("Player Info")] public static string UserName;
        public int playerScore;
        private bool isNewRecord = false;
        
        public static bool IsNewRecord { get; private set; }
        
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
        
        public static void SetUserName(string name)
            {
                UserName = name;
            }
        
        public void InitializeGame()
            {
                playerScore = 0;
            }
        
        public void SetPlayerAvatar(int index)
            {
                avatarIndex = index;
            }
        
        public int GetPlayerAvatar()
            {
                return avatarIndex;
            }
    }