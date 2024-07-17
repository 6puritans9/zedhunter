using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RotateItem : MonoBehaviourPunCallbacks, IPunObservable
{
    public float rotationSpeed = 50f;
    private PhotonView photonView;
    private Quaternion networkRotation;
    private float angle;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        networkRotation = transform.rotation;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // 월드 좌표계 기준으로 Y축 회전
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // 네트워크 동기화
            transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, angle * (1.0f / PhotonNetwork.SerializationRate));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkRotation = (Quaternion)stream.ReceiveNext();
            angle = Quaternion.Angle(transform.rotation, networkRotation);
        }
    }
}