using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
    {
        public Button startButton;

        private void onStartClick()
            {
                SceneManager.LoadScene("GameScene");
                
            }
}
