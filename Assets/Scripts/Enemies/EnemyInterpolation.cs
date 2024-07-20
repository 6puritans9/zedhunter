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

        PhotonNetwork.SerializationRate = 5; // 초당 패킷 직렬화 횟수 설정
        PhotonNetwork.SendRate = 10;          // 초당 네트워크 메시지 전송 횟수 설정
    }

    // Update is called once per frame
    void Update()
    {
        // The current gun object is not owned by the player, so it is inactive
        if (!photonView.IsMine)
        {
            // 추가 로직: 무기 장착시 애니메이션 등
            // 추가 로직: 무기 홀스터 시 애니메이션 등
            float lerpFactor = (Time.time - lastReceivedTime) * PhotonNetwork.SerializationRate;

            // 위치를 보간합니다.
            transform.position = Vector3.Lerp(lastPosition, targetPosition, lerpFactor);

            // 회전을 보간합니다.
            transform.rotation = Quaternion.Lerp(lastRotation, targetRotation, lerpFactor);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((transform.position));
            stream.SendNext((transform.rotation));
        }
        else
        {
            lastPosition = targetPosition;
            lastRotation = targetRotation;
            targetPosition = ((Vector3)stream.ReceiveNext());
            targetRotation = ((Quaternion)stream.ReceiveNext());
            lastReceivedTime = Time.time;
        }
    }

    // float 값을 압축하여 Vector3로 변환하는 함수입니다.
    private Vector3 CompressVector(Vector3 vector)
    {
        // 각 축의 값을 1000으로 곱하여 압축하고, -500 ~ 500 범위로 클램프합니다.
        return new Vector3(
            Mathf.Clamp(vector.x * 1000f, -500f, 500f),
            Mathf.Clamp(vector.y * 1000f, -500f, 500f),
            Mathf.Clamp(vector.z * 1000f, -500f, 500f));
    }

    // 압축된 Vector3 값을 원래의 Vector3 값으로 복원하는 함수입니다.
    private Vector3 DecompressVector(Vector3 compressed)
    {
        // 각 축의 값을 1000으로 나누어 원래의 좌표로 복원합니다.
        return new Vector3(compressed.x / 1000f, compressed.y / 1000f, compressed.z / 1000f);
    }

    // Quaternion을 Vector3로 압축하는 함수입니다.
    private Vector3 CompressQuaternion(Quaternion quaternion)
    {
        // Quaternion의 각도를 Euler 각도로 변환하여 100으로 곱해 압축합니다.
        Vector3 euler = quaternion.eulerAngles;
        return new Vector3(
            Mathf.Clamp(euler.x * 100f, -180f, 180f),
            Mathf.Clamp(euler.y * 100f, -180f, 180f),
            Mathf.Clamp(euler.z * 100f, -180f, 180f));
    }

    // 압축된 Vector3 값을 원래의 Quaternion 값으로 복원하는 함수입니다.
    private Quaternion DecompressQuaternion(Quaternion compressed)
    {
        // Euler 각도를 100으로 나누어 원래의 각도로 복원합니다.
        return Quaternion.Euler(compressed.x / 100f, compressed.y / 100f, compressed.z / 100f);
    }
}



// using Photon.Pun;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class EnemyInterpolation : MonoBehaviourPun, IPunObservable
// {
//     private Vector3 targetPosition;
//     private Quaternion targetRotation;
//
//     private Vector3 lastPosition;
//     private Quaternion lastRotation;
//
//     private float lastReceivedTime;
//
//     PhotonView photonView;
//
//     // Start is called before the first frame update
//     void Awake()
//     {
//         photonView = GetComponent<PhotonView>();
//         targetPosition = transform.position;
//         targetRotation = transform.rotation;
//         lastPosition = transform.position;
//         lastRotation = transform.rotation;
//         lastReceivedTime = Time.time;
//
//         PhotonNetwork.SerializationRate = 5; // 초당 패킷 직렬화 횟수 설정
//         PhotonNetwork.SendRate = 10;          // 초당 네트워크 메시지 전송 횟수 설정
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         // The current gun object is not owned by the player, so it is inactive
//         if (!photonView.IsMine)
//         {
//             // 추가 로직: 무기 장착시 애니메이션 등
//             // 추가 로직: 무기 홀스터 시 애니메이션 등
//             float lerpFactor = (Time.time - lastReceivedTime) * PhotonNetwork.SerializationRate;
//
//             // ������ ��ġ�� ������Ʈ�մϴ�.
//             transform.position = Vector3.Lerp(lastPosition, targetPosition, lerpFactor);
//
//             // ������ ȸ���� ������Ʈ�մϴ�.
//             transform.rotation = Quaternion.Lerp(lastRotation, targetRotation, lerpFactor);
//         }
//     }
//
//     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//     {
//         if (stream.IsWriting)
//         {
//             stream.SendNext((transform.position));
//             stream.SendNext((transform.rotation));
//         }
//         else
//         {
//             lastPosition = targetPosition;
//             lastRotation = targetRotation;
//             targetPosition = ((Vector3)stream.ReceiveNext());
//             targetRotation = ((Quaternion)stream.ReceiveNext());
//             lastReceivedTime = Time.time;
//         }
//     }
//
//     // float ���� �ϳ��� Vector3�� �����ϴ� �޼����Դϴ�.
//     private Vector3 CompressVector(Vector3 vector)
//     {
//         // �� ���� ���� 1000���Ͽ� ������ ��ȯ�ϰ�, -500 ~ 500 ������ ������ ��ȯ�մϴ�.
//         return new Vector3(
//             Mathf.Clamp(vector.x * 1000f, -500f, 500f),
//             Mathf.Clamp(vector.y * 1000f, -500f, 500f),
//             Mathf.Clamp(vector.z * 1000f, -500f, 500f));
//     }
//
//     // ����� Vector3 ���� ������ Vector3 ������ �����ϴ� �޼����Դϴ�.
//     private Vector3 DecompressVector(Vector3 compressed)
//     {
//         // �� ���� ���� 1000���� ������ ������ �Ҽ��� ��ġ�� �����մϴ�.
//         return new Vector3(compressed.x / 1000f, compressed.y / 1000f, compressed.z / 1000f);
//     }
//
//     // Quaternion�� Vector3�� �����ϴ� �޼����Դϴ�.
//     private Vector3 CompressQuaternion(Quaternion quaternion)
//     {
//         // Quaternion�� �� Euler ������ 100���Ͽ� ������ ��ȯ�մϴ�.
//         Vector3 euler = quaternion.eulerAngles;
//         return new Vector3(
//             Mathf.Clamp(euler.x * 100f, -180f, 180f),
//             Mathf.Clamp(euler.y * 100f, -180f, 180f),
//             Mathf.Clamp(euler.z * 100f, -180f, 180f));
//     }
//
//     // ����� Vector3 ���� ������ Quaternion ������ �����ϴ� �޼����Դϴ�.
//     private Quaternion DecompressQuaternion(Quaternion compressed)
//     {
//         // �� Euler ������ 100���� ������ ������ �Ҽ��� ��ġ�� �����մϴ�.
//         return Quaternion.Euler(compressed.x / 100f, compressed.y / 100f, compressed.z / 100f);
//     }
// }