using System;
using SlimUI.ModernMenu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour {
    public TMP_Text roomName;
    private LobbyManager _lobbyManager;
    private ToggleManager _toggleManager;
    private Button _button;
    
    public void Start() {
        _lobbyManager = FindObjectOfType<LobbyManager>();
        _toggleManager = GameObject.FindWithTag("ToggleManager")?.GetComponent<ToggleManager>();
        _button = GetComponent<Button>();
        if(_button != null)
            _button.onClick.AddListener(OnClickItem);
        }


    public void SetRoomName(string _roomName) {
        roomName.text = _roomName;
    }

    public void OnClickItem() {
        _lobbyManager.JoinRoom(roomName.text);
        _toggleManager.ToggleLobbyButtons();
    }
}
