using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSetup : MonoBehaviour
{
    public Bullet bullet;

    public void IsLocalBullet()
    {
        bullet.enabled = true;
    }
}
