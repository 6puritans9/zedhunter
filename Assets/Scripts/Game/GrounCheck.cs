using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrounCheck : MonoBehaviour
{
    public EnemyController enemyController;
    
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
			if (enemyController.agent && enemyController.agent.isOnNavMesh)
				enemyController.agent.isStopped = false;
             if(enemyController.animator)
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
			if (enemyController.agent && enemyController.agent.isOnNavMesh)
				enemyController.agent.isStopped = false;
             if (enemyController.animator)
                 enemyController.animator.SetBool("isJumping", false);
         }
     }
}
