using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour {
    public TMP_Text roomName;
    private LobbyManager _manager;

    public void Start() {
        _manager = FindObjectOfType<LobbyManager>();
        
    }


    public void SetRoomName(string _roomName) {
        roomName.text = _roomName;
    }

    public void OnClickItem() {
        _manager.JoinRoom(roomName.text);
    }
}
