using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [Header("Player Character Index")]
        [HideInInspector] public int avatarIndex;

        [Header("Player Info")] public static string UserName;
        [HideInInspector] public int playerScore;
        private bool isNewRecord = false;

        [Header("Scores")] private int PizzaZombieScore = 100;
        private int PlayerZombieScore = 200;
        private int BossZombieScore = 500;
        private int LostPizzaItemScore = -1000;
        
        public static bool IsNewRecord { get; private set; }
        
        private void Awake()
            {
                if (instance != null && instance != this)
                    {
                        Destroy(gameObject);
                        return;
                    }
                
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        
        public static void SetUserName(string name)
            {
                UserName = name;
            }
        
        public void InitializeGame()
            {
                playerScore = 0;
            }
        
        public void SetPlayerAvatar(int index)
            {
                avatarIndex = index;
            }
        
        public int GetPlayerAvatar()
            {
                return avatarIndex;
            }

        public void AddPizzaZombieScore()
            {
                playerScore += PizzaZombieScore;
            }
        
        public void AddPlayerZombieScore()
            {
                playerScore += PlayerZombieScore;
            }
        public void AddBossZombieScore()
            {
                playerScore += BossZombieScore;
            }

        public void SubtractPizzaItemScore()
            {
                playerScore += LostPizzaItemScore;
            }
    }