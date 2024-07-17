using UnityEngine;
using Photon.Pun;

public class ActionController : MonoBehaviourPunCallbacks
{
    private Animator animator;
    public float actionCooldown = 0.29f;
    private float lastActionTime = -Mathf.Infinity;
    private bool isFixing = false;
    private Coroutine fixingCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.F) && CanPerformAction())
        {
            photonView.RPC("StartFixingAction", RpcTarget.All);
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            photonView.RPC("StopFixingAction", RpcTarget.All);
        }
    }

    [PunRPC]
    void StartFixingAction()
    {
        if (!isFixing)
        {
            isFixing = true;
            lastActionTime = Time.time;
            animator.SetBool("isFixing", true);
            Debug.Log("Started Fixing Action at " + Time.time);
        }
    }

    bool CanPerformAction()
    {
        return Time.time - lastActionTime >= actionCooldown;
    }



    [PunRPC]
    void StopFixingAction()
    {
        if (isFixing)
        {
            isFixing = false;
            animator.SetBool("isFixing", false);
            Debug.Log("Stopped Fixing Action at " + Time.time);
        }
    }


}