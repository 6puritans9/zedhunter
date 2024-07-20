using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class LobbyManager : MonoBehaviourPunCallbacks
    {
        // Room Creation Section
        public TMP_InputField roomInputField;
        public GameObject lobbyPanel;
        public GameObject roomPanel;
        public TMP_Text roomName;

        // Lobby Listing
        public RoomItem roomItemPrefab;
        private List<RoomItem> roomItemsList = new List<RoomItem>();
        public Transform roomListing;

        // Lobby Refreshing
        public float timeBetweenUpdates = 1.5f;
        private float _nextUpdateTime;

        // Player Listing
        public PlayerItem playerItemPrefab;
        private List<PlayerItem> PlayerItemsList = new List<PlayerItem>();
        public Transform playerListing;

        // Ready and Play
        public GameObject startButton;

        // Claude
        private bool hasJoinedLobby = false;

        private void Update()
            {
                if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 1)
                    {
                        startButton.SetActive(true);
                    }
                else
                    {
                        startButton.SetActive(false);
                    }
            }

        public void JoinLobby()
            {
                if (PhotonNetwork.IsConnectedAndReady)
                    PhotonNetwork.JoinLobby();
                else
                    Debug.LogError("Cannot join lobby. Not connected and ready.");
            }

        public override void OnJoinedLobby()
            {
                hasJoinedLobby = true;
                // Initialize or reset lobby state here
                UpdateRoomList(new List<RoomInfo>());
            }

        private void UpdateRoomList(List<RoomInfo> list)
            {
                foreach (RoomItem item in roomItemsList)
                    {
                        Destroy(item.gameObject);
                    }

                roomItemsList.Clear();

                foreach (RoomInfo room in list)
                    {
                        RoomItem newRoom = Instantiate(roomItemPrefab, roomListing);
                        newRoom.SetRoomName(room.Name);
                        roomItemsList.Add(newRoom);
                    }
            }

        public void OnClickCreate()
            {
                if (roomInputField.text.Length > 0)
                    {
                        PhotonNetwork.CreateRoom(roomInputField.text,
                            new RoomOptions() { MaxPlayers = 3, BroadcastPropsChangeToAll = true });
                    }
            }

        #region Joining Room

        public void JoinRoom(string roomName)
            {
                PhotonNetwork.JoinRoom(roomName);
                print($"joined room: {roomName}");
            }

        public override void OnJoinedRoom()
            {
                lobbyPanel.SetActive(false);
                roomPanel.SetActive(true);
                roomName.text = $"{PhotonNetwork.CurrentRoom.Name}";
                UpdatePlayerList();
            }

        public override void OnPlayerEnteredRoom(Player newPlayer)
            {
                UpdatePlayerList();
            }

        #endregion

        #region Leaving Room

        public void OnClickLeaveRoom()
            {
                PhotonNetwork.LeaveRoom();
            }

        public override void OnLeftRoom()
            {
                roomPanel.SetActive(false);
                lobbyPanel.SetActive(true);
            }

        public override void OnPlayerLeftRoom(Player otherPlayer)
            {
                UpdatePlayerList();
            }

        #endregion

        #region UpdatePlyaerList

        void UpdatePlayerList()
            {
                ClearPlayerList();

                if (PhotonNetwork.CurrentRoom == null)
                    {
                        return;
                    }

                foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
                    {
                        PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerListing);
                        newPlayerItem.SetPlayerInfo(player.Value);
                        PlayerItemsList.Add(newPlayerItem);

                        if (player.Value == PhotonNetwork.LocalPlayer)
                            {
                                newPlayerItem.ApplyLocalChanges();
                            }
                    }
            }

        private void ClearPlayerList()
            {
                foreach (PlayerItem item in PlayerItemsList)
                    {
                        Destroy(item.gameObject);
                    }

                PlayerItemsList.Clear();
            }

        #endregion

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
            {
                if (Time.time >= _nextUpdateTime)
                    {
                        UpdateRoomList(roomList);
                        _nextUpdateTime = Time.time + timeBetweenUpdates;
                    }
            }

        public void OnClickStartButton()
            {
                PhotonNetwork.LoadLevel("GameScene");
            }
    }