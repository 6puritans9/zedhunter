using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    public float timerDuration = 301f; // 5 minutes
    private float timeRemaining;
    public GameObject zombie4Prefab;

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
        // 'Enemy' 태그를 가진 모든 오브젝트를 찾습니다
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // 각 적 오브젝트를 'zombie4' 프리팹으로 교체합니다
        foreach (GameObject enemy in enemies)
        {
            Vector3 position = enemy.transform.position;
            Quaternion rotation = enemy.transform.rotation;
            Destroy(enemy);
            Instantiate(zombie4Prefab, position, rotation);
        }

        // 4초 후에 씬을 전환합니다
        // Invoke("LoadEndingScene", 4f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
		SceneTransitionManager.Instance.DissolveToScene("EndingScene");
    }

    private void LoadEndingScene()
    {
        SceneManager.LoadScene("EndingScene");
    }
}