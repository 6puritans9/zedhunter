using System;
using System.Collections;
using System.Collections.Generic;
using SlimUI.ModernMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
    {
        private UIMenuManager _uiMenuManager; 
        private PlayerItem _playerItem;

        private void Start()
            {
                _playerItem = FindObjectOfType<PlayerItem>();
                _uiMenuManager = FindObjectOfType<UIMenuManager>();
            }

        public void OnConnectClick()
            {
                _uiMenuManager.Position2();
                _uiMenuManager.ReturnMenu();
            }

        public void OnStartClick()
            {
                int playerAvatarIndex = (int)_playerItem.playerProperties["avatar"];
                
                GameManager.instance.SetPlayerAvatar(playerAvatarIndex);
                GameManager.instance.InitializeGame();
                SceneManager.LoadScene("TutorialScene");
            }
}
