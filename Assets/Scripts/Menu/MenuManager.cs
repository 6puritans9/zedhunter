using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
    {
        private PlayerItem _playerItem;
        public Button startButton;

        private void Start()
            {
                _playerItem = FindObjectOfType<PlayerItem>();
            }

        public void OnStartClick()
            {
                int playerAvatarIndex = (int)_playerItem.playerProperties["avatar"];
                
                GameManager.instance.SetPlayerAvatar(playerAvatarIndex);
                GameManager.instance.InitializeGame();
                SceneManager.LoadScene("GameScene");
            }
}
