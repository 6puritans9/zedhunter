using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class WallBuilder : MonoBehaviourPun
{
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

    void Start()
    {
		photonView = GetComponent<PhotonView>();
		// 여기서 레이어를 확인하고 설정합니다.
		if (LayerMask.NameToLayer("Wall") == -1)
        {
            Debug.LogError("Wall layer does not exist. Please create it in Unity's Layer settings.");
        }
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

            // 벽 크기의 절반만큼 위로 올림
            float wallHeight = wallPrefab.GetComponent<Renderer>().bounds.size.y;
            buildPosition.y += wallHeight / 2;

            if (currentPreview == null)
            {
                currentPreview = Instantiate(wallPreviewPrefab, buildPosition, Quaternion.identity);
                // Preview의 레이어를 Default로 설정 (또는 다른 적절한 레이어)
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

            // 미리보기 색상 변경
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
                GameObject oldestWall = builtWalls.Dequeue();
                PhotonNetwork.Destroy(oldestWall);
            }

            builtWalls.Enqueue(newWall);
        }
    }

    bool IsOverlapping(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, wallPrefab.GetComponent<Renderer>().bounds.extents * 0.9f, Quaternion.identity, wallLayer);
        return colliders.Length > 0;
    }
}