using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("트리거엔터");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("콜리젼 엔터");
    }
}
