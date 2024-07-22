using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoBehaviour
    {
        [Header("Vignetting")] private Volume postProcessVolume;
        private Vignette vignette;

        private bool isCursorLocked = true;

        // Start is called before the first frame update
        void Start()
            {
                postProcessVolume = FindObjectOfType<Volume>();
                if (postProcessVolume != null && postProcessVolume.profile.TryGet(out vignette))
                    {
                        vignette.active = true;
                        vignette.color.Override(Color.red);
                    }

                SetCursorState(true);
            }

        // Update is called once per frame
        void Update()
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        // Toggle the cursor state
                        isCursorLocked = !isCursorLocked;
                        SetCursorState(isCursorLocked);
                    }
            }

        public void UpdateHealthEffect(int playerHealth, int maxPlayerHealth)
            {
                if (vignette != null)
                    {
                        float healthPercentage = (float)playerHealth / maxPlayerHealth;
                        float vignetteIntensity = Mathf.Lerp(1.5f, 0f, healthPercentage);
                        vignette.intensity.Override(vignetteIntensity);
                        /*Debug.Log($"Health: {playerHealth}, Intensity: {vignetteIntensity}");*/
                    }
                else
                    {
                        Debug.LogError("Vignette effect is null!");
                    }
            }

        private void SetCursorState(bool isLocked)
            {
                if (isLocked)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                else
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
            }
    }

// Cursor settings
// Cursor.lockState = CursorLockMode.Locked;
// Cursor.visible = false;