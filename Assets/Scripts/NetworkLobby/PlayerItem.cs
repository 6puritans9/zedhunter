using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerItem : MonoBehaviourPunCallbacks {
    public Image backgroundImage;
    public Color highlightColor;
    
    public TMP_Text playerName;
    public GameObject leftArrowButton;
    public GameObject rightArrowButton;

    private ExitGames.Client.Photon.Hashtable _playerProperties = new ExitGames.Client.Photon.Hashtable();
    public Image playerAvatar;
    public Sprite[] avatars;

    private Player _player;
    
    private void Awake() {
        backgroundImage = GetComponent<Image>();
    }

    private void Start() {
        backgroundImage = GetComponent<Image>();
    }
    
    public void SetPlayerInfo(Player player) {
        playerName.text = player.NickName;
        _player = player;
        UpdatePlayerItem(player);
    }

    public void ApplyLocalChanges() {
        print(highlightColor);
        backgroundImage.color = highlightColor;
        leftArrowButton.SetActive(true);
        rightArrowButton.SetActive(true);
    }

    public void OnClickLeftArrow() {
        if ((int)_playerProperties["playerAvatar"] == 0) {
            _playerProperties["playerAvatar"] = avatars.Length - 1;
        }
        else {
            _playerProperties["playerAvatar"] = (int)_playerProperties["playerAvatar"] - 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(_playerProperties);
    }
    
    public void OnClickRightArrow() {
        if ((int)_playerProperties["playerAvatar"] == avatars.Length - 1) {
            _playerProperties["playerAvatar"] = 0;
        }
        else {
            _playerProperties["playerAvatar"] = (int)_playerProperties["playerAvatar"] + 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(_playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable hashTable) {
        if (_player == targetPlayer) {
            UpdatePlayerItem(targetPlayer);
        }
    }

    public void UpdatePlayerItem(Player player) {
        if (player.CustomProperties.ContainsKey("playerAvatar")) {
            playerAvatar.sprite = avatars[(int)player.CustomProperties["playerAvatar"]];
            _playerProperties["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];
        }
        else {
            _playerProperties["playerAvatar"] = 0;
        }
    }
}
