using UnityEngine;
using Photon.Pun;
using System.Collections.Generic; 

public class ComputerInteraction : MonoBehaviourPunCallbacks
{
    public GameObject cylinderPrefab;
    public float interactionDistance = 3f; // 상호작용 가능 거리
    public float interactionTime = 3f; // F 키를 눌러야 하는 시간

    private bool isInRange = false;
    private float interactionTimer = 0f;

    private void Update()
    {
        if (!photonView.IsMine) return; // 로컬 플레이어만 처리

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= interactionDistance)
        {
            if (!isInRange)
            {
                isInRange = true;
                Debug.Log("Player in range of computer");
            }

            if (Input.GetKey(KeyCode.F))
            {
                interactionTimer += Time.deltaTime;
                Debug.Log($"Holding F key. Time: {interactionTimer}");

                if (interactionTimer >= interactionTime)
                {
                    Debug.Log("Interaction complete");
                    photonView.RPC("ReplaceComputerWithCylinder", RpcTarget.All);
                }
            }
            else
            {
                interactionTimer = 0f;
            }
        }
        else
        {
            if (isInRange)
            {
                isInRange = false;
                interactionTimer = 0f;
                Debug.Log("Player out of range");
            }
        }
    }

    [PunRPC]
    private void ReplaceComputerWithCylinder()
    {
        if (!PhotonNetwork.IsMasterClient) return;
    
        Debug.Log("Replacing computer with cylinder");
        if (cylinderPrefab != null)
        {
            Vector3 spawnPosition = transform.position;
            Quaternion spawnRotation = transform.rotation;
            
            GameObject cylinder = PhotonNetwork.Instantiate(cylinderPrefab.name, spawnPosition, spawnRotation);
            
            // Ensure CylinderInteraction is added and has a PhotonView
            CylinderInteraction cylinderInteraction = cylinder.GetComponent<CylinderInteraction>();
            if (cylinderInteraction == null)
            {
                cylinderInteraction = cylinder.AddComponent<CylinderInteraction>();
            }
            
            PhotonView cylinderView = cylinder.GetComponent<PhotonView>();
            if (cylinderView == null)
            {
                cylinderView = cylinder.AddComponent<PhotonView>();
            }
            
            cylinderView.ObservedComponents = new List<Component> { cylinderInteraction };
            cylinderView.Synchronization = ViewSynchronization.UnreliableOnChange;
    
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Cylinder prefab is not assigned");
        }
    }
}