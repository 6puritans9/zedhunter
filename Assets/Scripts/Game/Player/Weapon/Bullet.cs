using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToDestroy;
    [HideInInspector] public WeaponManager weapon;
    [HideInInspector] public Vector3 dir;

    [Header("VFX")]
    public GameObject hitVFX;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // if (collision.gameObject.GetComponentInParent<EnemyHealth>())
        {
            // EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
            // PhotonView enemyPhotonView = enemyHealth.gameObject.GetComponent<PhotonView>();
            // enemyPhotonView.RPC("TakeDamage", RpcTarget.All, weapon.damage);
            //
            // if(enemyHealth.health <= 0 && !enemyHealth.isDead)
            // {
            //     enemyHealth.isDead = true;
            // }
        }
        // PhotonNetwork.Instantiate(hitVFX.name, transform.position, Quaternion.identity);
		Destroy(this.gameObject);
	}
}
