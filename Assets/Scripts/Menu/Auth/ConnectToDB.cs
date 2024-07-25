using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectToDB : MonoBehaviour
    {
        private string getRankEndpoint = "http://52.78.204.71:13756/getRank";
        private string setRankEndpoint = "http://52.78.204.71:13756/setRank";

        private string userName;

        public EndingResponse rankDataList;

        // Start is called before the first frame update
        void Start()
            {
                userName = "bbb";
            }

        public IEnumerator TryGetRank()
            {
                using (UnityWebRequest webRequest = UnityWebRequest.Get(getRankEndpoint))
                    {
                        // Send the request and wait for a response
                        yield return webRequest.SendWebRequest();

                        // Check for network errors or HTTP errors
                        if (webRequest.result != UnityWebRequest.Result.Success)
                            {
                                Debug.LogError("Error: " + webRequest.error);
                            }
                        else
                            {
                                // Handle the response
                                Debug.Log("Received: " + webRequest.downloadHandler.text);

                                rankDataList = JsonUtility.FromJson<EndingResponse>(webRequest.downloadHandler.text);
                            }
                    }
            }

        public IEnumerator TrySetRank()
            {
                string username = GameManager.UserName;

                WWWForm form = new WWWForm();
                form.AddField("rUsername", username);

                using (UnityWebRequest webRequest = UnityWebRequest.Post(setRankEndpoint, form))
                    {
                        yield return webRequest.SendWebRequest();

                        if (webRequest.result != UnityWebRequest.Result.Success)
                            {
                                Debug.LogError("Error: " + webRequest.error);
                            }
                        else
                            {
                                Debug.Log("Rank set successfully.");
                            }
                    }
            }
    }