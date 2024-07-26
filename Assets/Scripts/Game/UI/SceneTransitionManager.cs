using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [SerializeField] private Image dissolveImage;
    [SerializeField] private float dissolveDuration = 4f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void DissolveToScene(string sceneName)
    {
        StartCoroutine(DissolveCoroutine(sceneName));
    }

    private IEnumerator DissolveCoroutine(string sceneName)
    {
        if (dissolveImage == null)
        {
            Debug.LogError("Dissolve Image is not assigned!");
            yield break;
        }

        dissolveImage.gameObject.SetActive(true);

        // 화면을 어둡게 만듦
        yield return StartCoroutine(FadeCoroutine(0f, 1f));

        // 씬 로드
        SceneManager.LoadScene(sceneName);

        // 새 씬이 로드될 때까지 기다림
        yield return null;

        // 화면을 밝게 만듦 (새 씬에서 실행됨)
        yield return StartCoroutine(FadeCoroutine(1f, 0f));

        dissolveImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeCoroutine(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveDuration)
        {
            if (dissolveImage == null) yield break;

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / dissolveDuration);
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            dissolveImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새 씬이 로드되면 dissolveImage를 최상위로 가져옴
        if (dissolveImage != null)
        {
            dissolveImage.transform.SetAsLastSibling();
        }
    }
}