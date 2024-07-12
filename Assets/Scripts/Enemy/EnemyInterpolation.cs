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

        PhotonNetwork.SerializationRate = 5; // �ʴ� ��Ŷ ���� Ƚ�� ����
        PhotonNetwork.SendRate = 10;          // �ʴ� ��Ʈ��ũ ������Ʈ Ƚ�� ����
    }

    // Update is called once per frame
    void Update()
    {
        // ���� �� ��ü�� ���� �÷��̾��� ���� �ƴ϶�� ����
        if (!photonView.IsMine)
        {
            // ���� ����� ���
            // ��Ʈ��ũ �޽����� ���� �ӵ��� ���� ���� ��ġ�� ��ǥ ��ġ ���̸� ����
            float lerpFactor = (Time.time - lastReceivedTime) * PhotonNetwork.SerializationRate;

            // ������ ��ġ�� ������Ʈ�մϴ�.
            transform.position = Vector3.Lerp(lastPosition, targetPosition, lerpFactor);

            // ������ ȸ���� ������Ʈ�մϴ�.
            transform.rotation = Quaternion.Lerp(lastRotation, targetRotation, lerpFactor);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ������ ���� (����� ������)
            stream.SendNext((transform.position));
            stream.SendNext((transform.rotation));
        }
        else
        {
            // ������ ���� �� ���� ����
            lastPosition = targetPosition;
            lastRotation = targetRotation;
            targetPosition = ((Vector3)stream.ReceiveNext());
            targetRotation = ((Quaternion)stream.ReceiveNext());
            lastReceivedTime = Time.time;
        }
    }

    // float ���� �ϳ��� Vector3�� �����ϴ� �޼����Դϴ�.
    private Vector3 CompressVector(Vector3 vector)
    {
        // �� ���� ���� 1000���Ͽ� ������ ��ȯ�ϰ�, -500 ~ 500 ������ ������ ��ȯ�մϴ�.
        return new Vector3(
            Mathf.Clamp(vector.x * 1000f, -500f, 500f),
            Mathf.Clamp(vector.y * 1000f, -500f, 500f),
            Mathf.Clamp(vector.z * 1000f, -500f, 500f));
    }

    // ����� Vector3 ���� ������ Vector3 ������ �����ϴ� �޼����Դϴ�.
    private Vector3 DecompressVector(Vector3 compressed)
    {
        // �� ���� ���� 1000���� ������ ������ �Ҽ��� ��ġ�� �����մϴ�.
        return new Vector3(compressed.x / 1000f, compressed.y / 1000f, compressed.z / 1000f);
    }

    // Quaternion�� Vector3�� �����ϴ� �޼����Դϴ�.
    private Vector3 CompressQuaternion(Quaternion quaternion)
    {
        // Quaternion�� �� Euler ������ 100���Ͽ� ������ ��ȯ�մϴ�.
        Vector3 euler = quaternion.eulerAngles;
        return new Vector3(
            Mathf.Clamp(euler.x * 100f, -180f, 180f),
            Mathf.Clamp(euler.y * 100f, -180f, 180f),
            Mathf.Clamp(euler.z * 100f, -180f, 180f));
    }

    // ����� Vector3 ���� ������ Quaternion ������ �����ϴ� �޼����Դϴ�.
    private Quaternion DecompressQuaternion(Quaternion compressed)
    {
        // �� Euler ������ 100���� ������ ������ �Ҽ��� ��ġ�� �����մϴ�.
        return Quaternion.Euler(compressed.x / 100f, compressed.y / 100f, compressed.z / 100f);
    }
}