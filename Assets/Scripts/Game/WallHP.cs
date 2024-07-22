using Photon.Pun;
using System;
using UnityEngine;

public class WallHP : MonoBehaviourPun
{
    public int HP = 100;
    public GameObject Player;

    public static event Action<GameObject> OnWallDestroyed;

    PhotonView photonVeiw;
    private void OnEnable()
    {
        photonVeiw = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            if (photonVeiw.IsMine)
            {
                OnWallDestroyed?.Invoke(this.gameObject); // 벽이 파괴될 때 이벤트 발생
            }
        }
    }
}
