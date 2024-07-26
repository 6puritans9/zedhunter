using System;
using System.Collections;
using System.Collections.Generic;
using SlimUI.ModernMenu;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
    {
        private UIMenuManager _uiMenuManager; 
        private PlayerItem _playerItem;
        private ImageDecoder _imageDecoder;

        [Header("SelectImageButton")] public TMP_Text selectButtonText;
        public RawImage selectButtonImage;

        [Header("UserInfo")] public TMP_Text loggedUserName;
        
        private void Start()
            {
                _playerItem = FindObjectOfType<PlayerItem>();
                _uiMenuManager = FindObjectOfType<UIMenuManager>();
                _imageDecoder = GetComponent<ImageDecoder>();
            }

        public void OnSelectClick()
            {
                float ALPHA = Mathf.Clamp01(1);
                Color color = selectButtonImage.color;
                
                selectButtonText.gameObject.SetActive(false);
                color.a = ALPHA;
                selectButtonImage.color = color;
            }

        public void ApplyUserInfo(string dbName, string dbImage)
            {
                loggedUserName.text = dbName;
                _imageDecoder.SetUserImage(dbImage);
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
