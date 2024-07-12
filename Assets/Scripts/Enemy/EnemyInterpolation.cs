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
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        lastReceivedTime = Time.time;

        PhotonNetwork.SerializationRate = 5; // 초당 패킷 전송 횟수 조정
        PhotonNetwork.SendRate = 10;          // 초당 네트워크 업데이트 횟수 조정
    }

    // Update is called once per frame
    void Update()
    {
        // 만약 이 객체가 현재 플레이어의 것이 아니라면 실행
        if (!photonView.IsMine)
        {
            // 보간 계수를 계산
            // 네트워크 메시지의 전송 속도에 따라 현재 위치와 목표 위치 사이를 보간
            float lerpFactor = (Time.time - lastReceivedTime) * PhotonNetwork.SerializationRate;

            // 보간된 위치를 업데이트합니다.
            transform.position = Vector3.Lerp(lastPosition, targetPosition, lerpFactor);

            // 보간된 회전을 업데이트합니다.
            transform.rotation = Quaternion.Lerp(lastRotation, targetRotation, lerpFactor);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터 전송 (압축된 데이터)
            stream.SendNext((transform.position));
            stream.SendNext((transform.rotation));
        }
        else
        {
            // 데이터 수신 및 압축 해제
            lastPosition = targetPosition;
            lastRotation = targetRotation;
            targetPosition = ((Vector3)stream.ReceiveNext());
            targetRotation = ((Quaternion)stream.ReceiveNext());
            lastReceivedTime = Time.time;
        }
    }

    // float 값을 하나의 Vector3로 압축하는 메서드입니다.
    private Vector3 CompressVector(Vector3 vector)
    {
        // 각 축의 값을 1000배하여 정수로 변환하고, -500 ~ 500 사이의 값으로 변환합니다.
        return new Vector3(
            Mathf.Clamp(vector.x * 1000f, -500f, 500f),
            Mathf.Clamp(vector.y * 1000f, -500f, 500f),
            Mathf.Clamp(vector.z * 1000f, -500f, 500f));
    }

    // 압축된 Vector3 값을 원래의 Vector3 값으로 복원하는 메서드입니다.
    private Vector3 DecompressVector(Vector3 compressed)
    {
        // 각 축의 값을 1000으로 나누어 원래의 소수점 위치를 복원합니다.
        return new Vector3(compressed.x / 1000f, compressed.y / 1000f, compressed.z / 1000f);
    }

    // Quaternion을 Vector3로 압축하는 메서드입니다.
    private Vector3 CompressQuaternion(Quaternion quaternion)
    {
        // Quaternion의 각 Euler 각도를 100배하여 정수로 변환합니다.
        Vector3 euler = quaternion.eulerAngles;
        return new Vector3(
            Mathf.Clamp(euler.x * 100f, -180f, 180f),
            Mathf.Clamp(euler.y * 100f, -180f, 180f),
            Mathf.Clamp(euler.z * 100f, -180f, 180f));
    }

    // 압축된 Vector3 값을 원래의 Quaternion 값으로 복원하는 메서드입니다.
    private Quaternion DecompressQuaternion(Quaternion compressed)
    {
        // 각 Euler 각도를 100으로 나누어 원래의 소수점 위치를 복원합니다.
        return Quaternion.Euler(compressed.x / 100f, compressed.y / 100f, compressed.z / 100f);
    }
}