using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
    {
        public TMP_Text timerText;
        public float timerDuration = 301f; // 5 minutes
        private float timeRemaining;

        void Start()
            {
                timeRemaining = timerDuration;
            }
        
        void Update()
            {
                if (timeRemaining > 0)
                    {
                        timeRemaining -= Time.deltaTime;
                        if (timeRemaining < 0)
                            {
                                timeRemaining = 0;
                                TimerEnded();
                            }

                        UpdateTimerText();
                    }
            }
        
        private void UpdateTimerText()
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            }

        private void TimerEnded()
            {
                SceneManager.LoadScene("EndingScene");
            }
    }