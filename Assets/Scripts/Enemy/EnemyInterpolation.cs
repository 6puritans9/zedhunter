using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInterpolation : MonoBehaviourPun, IPunObservable
{
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private float lastReceivedTime;

    PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        lastReceivedTime = Time.time;

        PhotonNetwork.SerializationRate = 10; // 초당 패킷 전송 횟수 조정
        PhotonNetwork.SendRate = 20;          // 초당 네트워크 업데이트 횟수 조정
    }

    // Update is called once per frame
    void Update()
    {
        if(!photonView.IsMine)
        {
            float lerpFactor = (Time.time - lastReceivedTime) / PhotonNetwork.SerializationRate;
            transform.position = Vector3.Lerp(lastPosition, targetPosition, lerpFactor);
            transform.rotation = Quaternion.Lerp(lastRotation, targetRotation, lerpFactor);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            lastPosition = targetPosition;
            lastRotation = targetRotation;
            targetPosition = (Vector3)stream.ReceiveNext();
            targetRotation = (Quaternion)stream.ReceiveNext();
            lastReceivedTime = Time.time;
        }
    }
}
