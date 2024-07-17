using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks {
    // Room Creation Section
    public TMP_InputField roomInputField;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public TMP_Text roomName;

    // Lobby Listing
    public RoomItem roomItemPrefab;
    private List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    // Lobby Refreshing
    public float timeBetweenUpdates = 1.5f;
    private float _nextUpdateTime;
    
    // Player Listing
    public PlayerItem playerItemPrefab;
    private List<PlayerItem> PlayerItemsList = new List<PlayerItem>();
    public Transform playerItemParent;
    
    // Ready and Play
    public GameObject playButton;
    
    
    // Start is called before the first frame update
    private void Start() {
        PhotonNetwork.JoinLobby();
    }

    private void Update() {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 3) {
            playButton.SetActive(true);
        }
        else {
            playButton.SetActive(false);
        }
    }

    public void OnClickPlayButton() {
        PhotonNetwork.LoadLevel("Game");
    }
    
    

    public void OnClickCreate() {
        if (roomInputField.text.Length > 0) {
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions(){MaxPlayers = 3, BroadcastPropsChangeToAll = true});
        }
    }

    public override void OnJoinedRoom() {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name}";
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        if (Time.time >= _nextUpdateTime) {
            UpdateRoomList(roomList);
            _nextUpdateTime = Time.time + timeBetweenUpdates;
        }
    }

    private void UpdateRoomList(List<RoomInfo> list) {
        foreach (RoomItem item in roomItemsList) {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list) {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName) {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    // Refreshing the lobby
    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinLobby();
    }
    
    // After JoinRoom()
    void UpdatePlayerList() {
        foreach (PlayerItem item in PlayerItemsList) {
            Destroy(item.gameObject);
        }
        PlayerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom == null) {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players) {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            // PlayerItemsList.Add(newPlayerItem);
            newPlayerItem.SetPlayerInfo(player.Value);

            if (player.Value.Equals(PhotonNetwork.LocalPlayer)) {
                newPlayerItem.ApplyLocalChanges();
            }
        }
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        UpdatePlayerList();
    }
}