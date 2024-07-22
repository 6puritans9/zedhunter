using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerItem : MonoBehaviourPunCallbacks
    {
        public TMP_Text characterName;
        public GameObject leftArrowButton;
        public GameObject rightArrowButton;
        public Image playerAvatar;
        public Sprite[] avatars;

        private int currentAvatarIndex = 0;
        private Hashtable _playerProperties = new Hashtable();


        private void Start()
            {
                SetPlayerInfo();
            }

        private void Update()
            {
                SetPlayerInfo();
            }

        public void SetPlayerInfo()
            {
                characterName.text = playerAvatar.sprite.name;

                if (_playerProperties["avatar"] == null)
                    _playerProperties["avatar"] = 0;
            }
        
        public void UpdatePlayerItem()
            {
                playerAvatar.sprite = avatars[(int)_playerProperties["avatar"]];
            }

        #region ClickArrows

        public void OnClickLeftArrow()
            {
                if ((int)_playerProperties["avatar"] == 0)
                    {
                        _playerProperties["avatar"] = avatars.Length - 1;
                    }
                else
                    {
                        _playerProperties["avatar"] = (int)_playerProperties["avatar"] - 1;
                    }

                UpdatePlayerItem();
            }

        public void OnClickRightArrow()
            {
                if ((int)_playerProperties["avatar"] == avatars.Length - 1)
                    {
                        _playerProperties["avatar"] = 0;
                    }
                else
                    {
                        _playerProperties["avatar"] = (int)_playerProperties["avatar"] + 1;
                    }

                UpdatePlayerItem();
            }

        #endregion
        
    }