using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
    {
        public TMP_Text anyKeyText;
        public BlinkingText blinkingText;

        void Update()
            {
                if (Input.anyKeyDown)
                    {
                        if (anyKeyText != null)
                            {
                                blinkingText.StopAllCoroutines();
                                blinkingText.anyKeyText.enabled = false;
                            }

                        SceneManager.LoadScene("GameScene");
                    }
            }
    }