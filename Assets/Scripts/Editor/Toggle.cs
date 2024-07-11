using UnityEditor;
using UnityEngine;

public class Toggle
{
    [MenuItem("Tools/Toggle Play Mode _%t")] // Ctrl + T or Cmd + T
    // MenuItem을 통해 에디터 내 Tools 메뉴에 하위 항목 생성
    // %는 윈도 Ctrl, 맥 Cmd에 대응
    // t는 키보드 t
    public static void TogglePlayMode()
    {
        EditorApplication.isPlaying = !EditorApplication.isPlaying;
    }
}