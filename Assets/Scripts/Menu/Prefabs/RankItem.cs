using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankItem : MonoBehaviour
    {
        public TMP_Text playerRank;
        public TMP_Text playerName;
        public TMP_Text playerScore;
        public TMP_Text newRecordText;
        public RawImage playerRawImage;
        public Image playerAvatar;

        private void Start()
            {
                UpdateNewRecordStatus();
            }

        // Method to update the new record status
        private void UpdateNewRecordStatus()
            {
                if (GameManager.IsNewRecord)
                    {
                        newRecordText.gameObject.SetActive(true);
                    }

                return;
            }

        // This method can be called whenever the UI needs to be refreshed
        public void RefreshUI(int rank, string name, int score, Texture rawImage)
            {
                playerRank.text = rank.ToString();
                playerName.text = name;
                playerScore.text = score.ToString();
                playerRawImage.texture = rawImage;
                // playerAvatar.sprite = avatar;

                // Update the new record status whenever the UI is refreshed
                UpdateNewRecordStatus();
            }
}
