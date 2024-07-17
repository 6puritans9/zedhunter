using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrounCheck : MonoBehaviour
{
    EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            enemyController.isJumpAnimating = false;
            enemyController.isJumping = false;
            enemyController.agent.isStopped = false;

            enemyController.animator.SetBool("isJumping", false);
        }
        if (other.TryGetComponent(out WallHP wall))
        {
            wall.TakeDamage(100);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            enemyController.isJumpAnimating = false;
            enemyController.isJumping = false;
            enemyController.agent.isStopped = false;

            enemyController.animator.SetBool("isJumping", false);
        }
    }
}
