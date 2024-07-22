using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManagementHelper : MonoBehaviour
    {
        void Start()
            {
                // Start the coroutine to unload all scenes except the current one
                StartCoroutine(UnloadAllScenesExceptCurrent());
            }

        private IEnumerator UnloadAllScenesExceptCurrent()
            {
                // Get the currently active scene
                Scene currentScene = SceneManager.GetActiveScene();
                string currentSceneName = currentScene.name;

                // Loop through all loaded scenes
                for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        Scene scene = SceneManager.GetSceneAt(i);

                        // Skip the current active scene
                        if (scene.name == currentSceneName)
                            continue;

                        // Unload the scene asynchronously
                        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
                        yield return asyncUnload;
                    }

                Debug.Log("Unloaded all scenes except the current one.");
            }
    }