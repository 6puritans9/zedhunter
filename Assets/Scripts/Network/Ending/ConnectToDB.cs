using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectToDB : MonoBehaviour
    {
        private string setRankEndpoint = "http://52.78.204.71:13756/setRank";
        private string getRankEndpoint = "http://52.78.204.71:13756/getRank";

        private GameManager _gameManager;
        
        [HideInInspector] public EndingResponse rankDataList;

        // Start is called before the first frame update
        void Start()
            {
                _gameManager = FindObjectOfType<GameManager>();
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

                var jsonObject = new RankData
                    {
                        username = GameManager.UserName,
                        score = _gameManager.playerScore
                    };

                string jsonData = JsonUtility.ToJson(jsonObject);
                byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
                
                UnityWebRequest request = new UnityWebRequest(setRankEndpoint, "POST");
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                    {
                        Debug.LogError(request.error);
                    }
                else
                    {
                        Debug.Log(request.downloadHandler.text);
                    }
            }
        
        [System.Serializable]
        public class RankData
            {
                public string username;
                public int score;
            }
    }