using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    private int playersInCylinder = 0;
    private PhotonView pv; // PhotonView 컴포넌트를 위한 변수 추가

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }

        pv = GetComponent<PhotonView>(); // PhotonView 컴포넌트 가져오기
    }

    public void PlayerEnteredCylinder()
    {
        playersInCylinder++;
        CheckWinCondition();
    }

    public void PlayerExitedCylinder()
    {
        playersInCylinder--;
    }

    private void CheckWinCondition()
    {
        if (playersInCylinder == 3 && PhotonNetwork.IsMasterClient)
        {
            pv.RPC("ChangeScene", RpcTarget.All); // photonView 대신 pv 사용
        }
    }

    [PunRPC]
    private void ChangeScene()
    {
        // 다음 씬으로 전환하는 로직
        PhotonNetwork.LoadLevel("Ending");
    }
}