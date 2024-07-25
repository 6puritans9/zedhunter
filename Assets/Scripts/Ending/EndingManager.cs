using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
    {
        private ConnectToDB _database;
        public GameObject rankContainer;
        public RankItem rankItemPrefab;
    
        // Start is called before the first frame update
    void Start()
        {
            _database = GetComponent<ConnectToDB>();
            StartCoroutine(InitializeRankItems());
        }

    private IEnumerator InitializeRankItems()
        {
            yield return StartCoroutine(_database.TryGetRank());
            
            SetRankItems();
        }
    
    private void SetRankItems()
        {
            int rank = 1;
            
            foreach (GetRankResponse rankData in _database.rankDataList.data)
                {
                    RankItem rankItem = Instantiate(rankItemPrefab, rankContainer.transform);
                    Texture2D userImage = Base64ToTexture(rankData.userImage);  
                    
                    rankItem.RefreshUI(rank, rankData.username, rankData.score, userImage);

                    rank += 1;
                }
        }
    
    private Texture2D Base64ToTexture(string base64String)
        {
            byte[] imageBytes = System.Convert.FromBase64String(base64String);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(imageBytes);
            return texture;
        }
}
