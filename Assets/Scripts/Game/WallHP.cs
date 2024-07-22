using System;
using UnityEngine;

public class WallHP : MonoBehaviour
    {
        public int HP = 100;
        public GameObject Player;

        public static event Action<GameObject> OnWallDestroyed;


        // private void OnEnable()
        //     {
        //         photonVeiw = GetComponent<PhotonView>();
        //     }

        public void TakeDamage(int damage)
            {
                HP -= damage;
                if (HP <= 0)
                    {
                        OnWallDestroyed?.Invoke(this.gameObject); // ���� �ı��� �� �̺�Ʈ �߻�
                    }
            }
    }