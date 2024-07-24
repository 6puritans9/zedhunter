using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    // public Button menuButton;

    // void Start()
    // {
    //     // 버튼에 리스너 추가
    //     menuButton.onClick.AddListener(ReturnToMenuScene);
    // }

    public void ReturnToMenuScene()
    {
        // MenuScene으로 전환
        SceneManager.LoadScene("MenuScene");
    }
}