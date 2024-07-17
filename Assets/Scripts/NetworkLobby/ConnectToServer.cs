using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks {
    public TMP_InputField userNameInput;
    public TMP_Text buttonText;

    public void OnClickConnect() {
        if (userNameInput.text.Length > 0) {
            PhotonNetwork.NickName = userNameInput.text;
            buttonText.text = "Connecting...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster() {
        SceneManager.LoadScene("Lobby");
    }
}