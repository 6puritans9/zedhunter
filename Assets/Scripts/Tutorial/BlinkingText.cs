using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlinkingText : MonoBehaviour
{
    public TMP_Text anyKeyText; // Reference to the Text component
    public float blinkInterval = 0.5f; // Time interval for blinking

    private bool isBlinking = true;
    void Start()
    {
        if (anyKeyText == null)
            {
                anyKeyText = GetComponent<TMP_Text>();
            }

        StartCoroutine(BlinkText());
    }

    IEnumerator BlinkText()
        {
            while (true)
                {
                    // Toggle the visibility
                    anyKeyText.enabled = isBlinking;
                    isBlinking = !isBlinking;

                    // Wait for the specified interval
                    yield return new WaitForSeconds(blinkInterval);
                }
        }
}
