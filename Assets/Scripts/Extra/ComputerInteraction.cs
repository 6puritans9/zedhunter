

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class ComputerInteraction : MonoBehaviour
{
    public GameObject cylinderPrefab;
    public float interactionDistance = 3f;
    public float interactionTime = 3f;
    public GameObject progressBarPrefab; // 프리팹을 위한 GameObject 타입
    public float progressDecreaseRate = 0.3f; // 프로그레스 바 감소 속도

    private bool isInRange = false;
    private float interactionProgress = 0f;

    private static int interactedComputersCount = 0;
    private static readonly object lockObject = new object();
    private bool hasInteracted = false;
    private Slider progressBar;
    private Image fillImage;
    public Image progressImage;
    public Sprite[] progressSprites;

    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        CreateProgressBar();
        if (progressImage != null)
        {
            progressImage.type = Image.Type.Simple;
            progressImage.preserveAspect = true;
        }
        UpdateProgressImage();
    }

    private void CreateProgressBar()
    {
        // Canvas와 ProgressBar를 프리팹으로 생성
        GameObject canvasObject = new GameObject("ProgressBarCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.sortingOrder = 10;
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        // Canvas를 현재 오브젝트의 자식으로 설정
        canvasObject.transform.SetParent(transform, false);
        canvasObject.transform.localPosition = new Vector3(0, 4, 0); // 위치 조정
        canvasObject.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f); // 크기 조정

        // ProgressBar 프리팹을 인스턴스화하여 추가
        GameObject progressBarObject = Instantiate(progressBarPrefab, canvasObject.transform);
        progressBar = progressBarObject.GetComponent<Slider>();

        // Fill Image를 가져오기
        fillImage = progressBar.fillRect.GetComponent<Image>();

        // 크기 조정
        progressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 20);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= interactionDistance && !hasInteracted)
        {
            if (!isInRange)
            {
                isInRange = true;
                Debug.Log("Player in range of computer");
            }

            if (Input.GetKey(KeyCode.F))
            {
                interactionProgress += Time.deltaTime / interactionTime;
                interactionProgress = Mathf.Clamp01(interactionProgress);

                if (interactionProgress >= 1f)
                {
                    Debug.Log("Interaction complete");
                    InteractWithComputer();
                    SetProgressBarColor(Color.green); // 녹색으로 변경
                }
            }
            else
            {
                DecreaseProgress();
            }
        }
        else
        {
            if (isInRange)
            {
                isInRange = false;
                Debug.Log("Player out of range");
            }
            if (!hasInteracted) // hasInteracted가 true일 때만 프로그레스 바를 감소시키지 않음
            {
                DecreaseProgress();
            }
        }

        // 프로그레스 바 직접 업데이트
        if (progressBar != null)
        {
            progressBar.value = interactionProgress;
        }

        // 디버그 로그 추가
        Debug.Log($"Progress: {interactionProgress}");
        if (progressBar != null)
        {
            Debug.Log($"ProgressBar value: {progressBar.value}");
        }
        else
        {
            Debug.LogError("ProgressBar is null!");
        }

        photonView.RPC("SyncProgress", RpcTarget.All, interactionProgress);

        if (progressBar != null)
        {
            progressBar.transform.LookAt(Camera.main.transform);
            progressBar.transform.Rotate(0, 180, 0); // 방향 보정
        }
    }

    private void DecreaseProgress()
    {
        interactionProgress -= progressDecreaseRate * Time.deltaTime;
        interactionProgress = Mathf.Clamp01(interactionProgress);
    }

    private void SetProgressBarColor(Color color)
    {
        if (fillImage != null)
        {
            fillImage.color = color;
        }
    }

    [PunRPC]
    private void SyncProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
            if (progress >= 1f)
            {
                SetProgressBarColor(Color.green);
            }
        }

        interactionProgress = progress;
    }

    [PunRPC]
    private void InteractWithComputer()
    {
        if (hasInteracted) return;

        hasInteracted = true;
        IncrementInteractionCount();
    Debug.Log("Calling UpdateProgressImage");
        UpdateProgressImage();
    }

    private void IncrementInteractionCount()
    {
        lock (lockObject)
        {
            interactedComputersCount++;
            Debug.Log($"Interacted computers: {interactedComputersCount}");

            if (interactedComputersCount >= 3 && PhotonNetwork.IsMasterClient)
            {
                SpawnCylinder();
            }

        }
    }

    private void SpawnCylinder()
    {
        Debug.Log("Spawning cylinder");
        if (cylinderPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(23.5f, 100f, 32f);
            Quaternion spawnRotation = Quaternion.identity;

            PhotonNetwork.InstantiateRoomObject(cylinderPrefab.name, spawnPosition, spawnRotation);
        }
        else
        {
            Debug.LogError("Cylinder prefab is not assigned");
        }
    }

    private void UpdateProgressImage()
    {
        if (progressImage != null && progressSprites.Length >= 4)
        {
            int spriteIndex = 0;
            switch (interactedComputersCount)
            {
                case 0:
                    spriteIndex = 0; // 00.png
                    break;
                case 1:
                    spriteIndex = 1; // 30.png
                    break;
                case 2:
                    spriteIndex = 2; // 60.png
                    break;
                case 3:
                default:
                    spriteIndex = 3; // 100.png
                    break;
            }
        Debug.Log($"Updating progress image to sprite index: {spriteIndex}");
        progressImage.sprite = progressSprites[spriteIndex];
    }
    else
    {
        Debug.LogError("Progress image or progress sprites not properly set");
    }
    }
}
