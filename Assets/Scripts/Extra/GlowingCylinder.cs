using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowingCylinder : MonoBehaviour
{
    public Color glowColor = Color.white;
    public float glowIntensity = 2f;
    public float lightIntensity = 1f;

    private Renderer cylinderRenderer;
    private Light glowLight;

    void Start()
    {
        cylinderRenderer = GetComponent<Renderer>();
        SetGlow(glowColor, glowIntensity);

        // 실린더에 Light 컴포넌트 추가
        glowLight = gameObject.AddComponent<Light>();
        glowLight.type = LightType.Point;
        glowLight.color = glowColor;
        glowLight.intensity = lightIntensity;
    }

    public void SetGlow(Color color, float intensity)
    {
        cylinderRenderer.material.SetColor("_EmissionColor", color * intensity);
        cylinderRenderer.material.EnableKeyword("_EMISSION");

        if (glowLight != null)
        {
            glowLight.color = color;
            glowLight.intensity = intensity;
        }
    }
}