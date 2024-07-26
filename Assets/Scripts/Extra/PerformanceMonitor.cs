using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class PerformanceMonitor : MonoBehaviour
{
    public TMP_Text fpsText;
    public TMP_Text gcText;

    private int frameCount;
    private float nextUpdate;
    private float fps;
    private const float updateRate = 4.0f;  // 업데이트 주기 (4번/초)

    private long lastGCCount;

    void Start()
    {
        nextUpdate = Time.time;
        lastGCCount = GC.CollectionCount(0);
    }

    void Update()
    {
        frameCount++;
        if (Time.time >= nextUpdate)
        {
            fps = frameCount / (Time.time - nextUpdate + 1 / updateRate);
            frameCount = 0;
            nextUpdate += 1.0f / updateRate;

            UpdateUI();
        }
    }

    void UpdateUI()
    {
        fpsText.text = "FPS: " + fps.ToString("F2");
        gcText.text = "GC Count: " + (GC.CollectionCount(0) - lastGCCount).ToString();
    }
}
