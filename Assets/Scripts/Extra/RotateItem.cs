using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateItem : MonoBehaviour
    {
        public float rotationSpeed = 50f;
        private Quaternion networkRotation;
        private float angle;

        void Awake()
            {
                networkRotation = transform.rotation;
            }

        void Update()
            {
                // 월드 좌표계 기준으로 Y축 회전
                transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
            }

        // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        // {
        //     if (stream.IsWriting)
        //     {
        //         stream.SendNext(transform.rotation);
        //     }
        //     else
        //     {
        //         networkRotation = (Quaternion)stream.ReceiveNext();
        //         angle = Quaternion.Angle(transform.rotation, networkRotation);
        //     }
        // }
    }