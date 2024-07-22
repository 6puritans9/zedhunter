using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoBehaviour
{
    [Header("Vignetting")]
    private Volume postProcessVolume;
    private Vignette vignette;
    
    // Start is called before the first frame update
    void Start()
    {
        postProcessVolume = FindObjectOfType<Volume>();
        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out vignette))
            {
                vignette.active = true;
                vignette.color.Override(Color.red);
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void UpdateHealthEffect(int playerHealth, int maxPlayerHealth)
        {
            if (vignette != null)
                {
                    float healthPercentage = (float)playerHealth / maxPlayerHealth;
                    float vignetteIntensity = Mathf.Lerp(1.5f, 0f, healthPercentage);
                    vignette.intensity.Override(vignetteIntensity);
                    Debug.Log($"Health: {playerHealth}, Intensity: {vignetteIntensity}");
                }
            else
                {
                    Debug.LogError("Vignette effect is null!");
                }
        }
}
