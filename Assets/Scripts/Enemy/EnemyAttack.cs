using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ʈ���ſ���");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("�ݸ��� ����");
    }
}
