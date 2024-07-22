using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerItem : MonoBehaviour
    {
        public TMP_Text characterName;
        public Image playerAvatar;
        public Sprite[] avatars;
        
        public GameObject leftArrowButton;
        public GameObject rightArrowButton;

        [HideInInspector] public Hashtable playerProperties = new Hashtable();


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

                if (playerProperties["avatar"] == null)
                    playerProperties["avatar"] = 0;
            }
        
        public void UpdatePlayerItem()
            {
                playerAvatar.sprite = avatars[(int)playerProperties["avatar"]];
            }

        #region ClickArrows

        public void OnClickLeftArrow()
            {
                if ((int)playerProperties["avatar"] == 0)
                    {
                        playerProperties["avatar"] = avatars.Length - 1;
                    }
                else
                    {
                        playerProperties["avatar"] = (int)playerProperties["avatar"] - 1;
                    }

                UpdatePlayerItem();
            }

        public void OnClickRightArrow()
            {
                if ((int)playerProperties["avatar"] == avatars.Length - 1)
                    {
                        playerProperties["avatar"] = 0;
                    }
                else
                    {
                        playerProperties["avatar"] = (int)playerProperties["avatar"] + 1;
                    }

                UpdatePlayerItem();
            }

        #endregion
        
    }