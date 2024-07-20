using Photon.Pun;
using SlimUI.ModernMenu;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        public UIMenuManager _uiMenuManager;
        public TMP_Text playerWelcome;
        private LobbyManager _LobbyManager;

        public void OnClickConnect(string userNameInput)
            {
                PhotonNetwork.NickName = userNameInput;
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.ConnectUsingSettings();
            }

        public override void OnConnectedToMaster()
            {
                _LobbyManager = FindObjectOfType<LobbyManager>();
                
                _uiMenuManager.Position2();
                _uiMenuManager.ReturnMenu();
                playerWelcome.text = $"Welcome, {PhotonNetwork.NickName}";
                
                _LobbyManager.JoinLobby();
            }
        
        public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
            {
                Debug.LogError($"Disconnected from Photon server: {cause}");
                // Handle disconnection, show error message to the user, etc.
            }
    }