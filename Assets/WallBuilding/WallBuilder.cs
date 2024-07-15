using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.Linq; // List 변환을 위해 추가

public class WallBuilder : MonoBehaviourPun
{
    public static WallBuilder Instance;

    PhotonView photonView;

    public GameObject wallPrefab;
    public GameObject wallPreviewPrefab;
    public LayerMask buildableLayer;
    public LayerMask wallLayer;
    public int maxWalls = 3;

    private GameObject currentPreview;
    private bool isBuildingEnabled = false;
    private Queue<GameObject> builtWalls = new Queue<GameObject>();
    private Vector3 lastValidPosition;

    private void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (LayerMask.NameToLayer("Wall") == -1)
        {
            Debug.LogError("Wall layer does not exist. Please create it in Unity's Layer settings.");
        }

        WallHP.OnWallDestroyed += HandleWallDestroyed; // 이벤트 구독
    }

    void OnDestroy()
    {
        WallHP.OnWallDestroyed -= HandleWallDestroyed; // 이벤트 구독 해제
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleBuildMode();
            }

            if (isBuildingEnabled)
            {
                UpdatePreview();

                if (Input.GetMouseButtonDown(0))
                {
                    BuildWall();
                }
            }
            else if (currentPreview != null)
            {
                Destroy(currentPreview);
                currentPreview = null;
            }
        }
    }

    void ToggleBuildMode()
    {
        isBuildingEnabled = !isBuildingEnabled;
        Debug.Log("Building mode: " + (isBuildingEnabled ? "On" : "Off"));
    }

    void UpdatePreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, buildableLayer))
        {
            Vector3 buildPosition = hit.point;
            buildPosition.y = Mathf.Round(buildPosition.y);

            float wallHeight = wallPrefab.GetComponent<Renderer>().bounds.size.y;
            buildPosition.y += wallHeight / 2;

            if (currentPreview == null)
            {
                currentPreview = Instantiate(wallPreviewPrefab, buildPosition, Quaternion.identity);
                currentPreview.layer = LayerMask.NameToLayer("PreviewWall");
            }

            bool canBuild = !IsOverlapping(buildPosition);

            if (canBuild)
            {
                lastValidPosition = buildPosition;
                currentPreview.transform.position = buildPosition;
            }
            else
            {
                currentPreview.transform.position = lastValidPosition;
            }

            Renderer previewRenderer = currentPreview.GetComponent<Renderer>();
            if (previewRenderer != null)
            {
                previewRenderer.material.color = canBuild ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
            }
        }
    }

    void BuildWall()
    {
        if (currentPreview != null && !IsOverlapping(currentPreview.transform.position))
        {
            GameObject newWall = PhotonNetwork.Instantiate(wallPrefab.name, currentPreview.transform.position, Quaternion.identity);
            newWall.layer = LayerMask.NameToLayer("Wall");

            if (builtWalls.Count >= maxWalls)
            {
                photonView.RPC("DestroyBlock", RpcTarget.All);
            }

            builtWalls.Enqueue(newWall);
        }
    }

    public void UseRPC_AddBlock()
    {
        GameObject newWall = PhotonNetwork.Instantiate(wallPrefab.name, currentPreview.transform.position, Quaternion.identity);
        newWall.layer = LayerMask.NameToLayer("Wall");
        newWall.SetActive(false);
        builtWalls.Enqueue(newWall);
    }

    public void UseRPC_DestroyBlock()
    {
        photonView.RPC("DestroyBlock", RpcTarget.All);
    }

    [PunRPC]
    public void DestroyBlock()
    {
        GameObject oldestWall = builtWalls.Dequeue();
        PhotonNetwork.Destroy(oldestWall);
    }

    bool IsOverlapping(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, wallPrefab.GetComponent<Renderer>().bounds.extents * 0.9f, Quaternion.identity, wallLayer);
        return colliders.Length > 0;
    }

    private void HandleWallDestroyed(GameObject wall)
    {
        if (builtWalls.Contains(wall))
        {
            // Queue를 List로 변환
            List<GameObject> tempList = builtWalls.ToList();
            // List에서 제거
            tempList.Remove(wall);
            // 다시 Queue로 변환
            builtWalls = new Queue<GameObject>(tempList);
            Debug.Log("Wall destroyed and removed from queue.");
        }
        PhotonNetwork.Destroy(wall);
    }
}
