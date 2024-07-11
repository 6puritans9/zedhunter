using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySetup : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public void EnemySet()
    {
        enemyHealth.enabled = true;
    }
}
