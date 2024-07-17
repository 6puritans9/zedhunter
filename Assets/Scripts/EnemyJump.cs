using UnityEngine;
using Photon.Pun;
using System.Collections;

public class EnemyJump : MonoBehaviourPun
{
    public Transform playerTarget; // 플레이어 타겟
    public float jumpHeight = 5f; // 점프 높이
    public float duration = 1f; // 점프 지속 시간

    private Vector3 startPoint;
    private Vector3 targetPoint;
    private bool isJumping = false;
    private float jumpProgress = 0f;
    private Vector3 networkPosition;

    private void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(CheckForPlayer());
        }
    }

    private void Update()
    {
        if (isJumping)
        {
            Jump();
        }
        else if (!photonView.IsMine)
        {
            SmoothMove();
        }
    }

    IEnumerator CheckForPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            if (!isJumping && Vector3.Distance(transform.position, playerTarget.position) < 10f)
            {
                StartJump();
            }
        }
    }

    void StartJump()
    {
        startPoint = transform.position;
        targetPoint = playerTarget.position;
        isJumping = true;
        jumpProgress = 0f;
        photonView.RPC("SyncStartJump", RpcTarget.Others, startPoint, targetPoint);
    }

    [PunRPC]
    void SyncStartJump(Vector3 start, Vector3 target)
    {
        startPoint = start;
        targetPoint = target;
        isJumping = true;
        jumpProgress = 0f;
    }

    void Jump()
    {
        jumpProgress += Time.deltaTime / duration;
        if (jumpProgress >= 1f)
        {
            jumpProgress = 1f;
            isJumping = false;
        }

        Vector3 currentPos = Vector3.Lerp(startPoint, targetPoint, jumpProgress);
        currentPos.y += Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight;

        transform.position = currentPos;
    }

    void SmoothMove()
    {
        transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
        }
    }
}



