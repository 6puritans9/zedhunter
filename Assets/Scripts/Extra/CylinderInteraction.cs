using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class CylinderInteraction : MonoBehaviourPunCallbacks, IPunObservable
{
    private HashSet<int> playersInContact = new HashSet<int>();
    private const int REQUIRED_PLAYERS = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        PhotonView playerView = other.GetComponent<PhotonView>();
        if (playerView != null && other.CompareTag("Player"))
        {
            Debug.Log($"Player entered: {playerView.ViewID}");
            photonView.RPC("AddPlayer", RpcTarget.All, playerView.ViewID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine) return;

        PhotonView playerView = other.GetComponent<PhotonView>();
        if (playerView != null && other.CompareTag("Player"))
        {
            Debug.Log($"Player exited: {playerView.ViewID}");
            photonView.RPC("RemovePlayer", RpcTarget.All, playerView.ViewID);
        }
    }

    [PunRPC]
    private void AddPlayer(int viewId)
    {
        playersInContact.Add(viewId);
        Debug.Log($"Players in contact: {playersInContact.Count}");
        CheckPlayerCount();
    }

    [PunRPC]
    private void RemovePlayer(int viewId)
    {
        playersInContact.Remove(viewId);
        Debug.Log($"Players in contact: {playersInContact.Count}");
    }

    private void CheckPlayerCount()
    {
        if (playersInContact.Count >= REQUIRED_PLAYERS && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ChangeScene", RpcTarget.All);
        }
    }

    [PunRPC]
    private void ChangeScene()
    {
        Debug.Log("Changing scene...");
        // 여기에 씬 전환 로직을 구현하세요.
        PhotonNetwork.LoadLevel("Ending");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playersInContact.Count);
        }
        else
        {
            int count = (int)stream.ReceiveNext();
            Debug.Log($"Received player count: {count}");
        }
    }
}