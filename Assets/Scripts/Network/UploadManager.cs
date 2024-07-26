using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UploadManager : MonoBehaviour
    {
        private ConnectToDB _database;
        private GameManager _gameManager;

        // Start is called before the first frame update
        public void Connect()
            {
                _gameManager = FindObjectOfType<GameManager>();
                _database = GetComponent<ConnectToDB>();

                StartCoroutine(SetRankItems());
            }

        private IEnumerator SetRankItems()
            {
                yield return StartCoroutine(_database.TrySetRank());
            }
    }