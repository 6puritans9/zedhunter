using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionStateManager : MonoBehaviourPunCallbacks
{
    [HideInInspector] public ActionBaseState currentState;

    public DefaultState Default = new DefaultState();
    public ReloadState Reload = new ReloadState();
    public SwordAction SwordAction = new SwordAction();

    public bool isDead;
    public int playerHealth = 100;
    public GameObject currentWeapon;
    [HideInInspector] public WeaponAmmo ammo;
    AudioSource audioSource;

    [HideInInspector] public Animator anim;

    public MultiAimConstraint rHandAim;
    public TwoBoneIKConstraint lHandIK;

    public GameObject gun;
    public Rig gunRig;

    public float cooldownTime = 2f;
    public float nextFireTime = 0f;
    public static int noOfClicks = 0;
    public float lastClickedTime = 0;
    public float maxComboDelay = 1;

    public PhotonView playerSetupView;

    private void Awake()
    {
        Debug.Log("ActionStateManager: Awake called");
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("ActionStateManager: Animator component not found!");
        }
    }

    void Start()
    {
        Debug.Log("ActionStateManager: Start called");
        
        if (!photonView.IsMine)
        {
            Debug.Log("ActionStateManager: This is not the local player's instance");
            return;
        }

        SwitchState(Default);
        isDead = false;

        if (gun == null)
        {
            Debug.LogError("ActionStateManager: Gun object is null!");
        }
        else
        {
            gun.SetActive(false);
        }

        if (gunRig == null)
        {
            Debug.LogError("ActionStateManager: GunRig is null!");
        }
        else
        {
            gunRig.weight = 0;
        }

        SetLayerWeight(0, 1);  // Sword layer

        if (playerSetupView == null)
        {
            Debug.LogError("ActionStateManager: playerSetupView is null!");
        }
        else
        {
            playerSetupView.RPC("SetTPWeapon", RpcTarget.All, 1);
        }

        if (currentWeapon == null)
        {
            Debug.LogError("ActionStateManager: currentWeapon is null!");
        }
        else
        {
            ammo = currentWeapon.GetComponent<WeaponAmmo>();
            if (ammo == null)
            {
                Debug.LogError("ActionStateManager: WeaponAmmo component not found on currentWeapon!");
            }

            audioSource = currentWeapon.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("ActionStateManager: AudioSource component not found on currentWeapon!");
            }
        }
        
        SwitchToGun();
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     SwitchToSword();
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     SwitchToGun();
        // }

        if (currentState == null)
        {
            Debug.LogError("ActionStateManager: currentState is null!");
        }
        else
        {
            currentState.UpdateState(this);
        }
    }

    void SwitchToSword()
    {
        Debug.Log("ActionStateManager: Switching to Sword");
        if (playerSetupView == null)
        {
            Debug.LogError("ActionStateManager: playerSetupView is null when switching to sword!");
        }
        else
        {
            playerSetupView.RPC("SetTPWeapon", RpcTarget.All, 1);
        }

        if (gun == null)
        {
            Debug.LogError("ActionStateManager: gun is null when switching to sword!");
        }
        else
        {
            gun.SetActive(false);
        }

        SetLayerWeight(0, 1);  // Sword layer
    }

    void SwitchToGun()
    {
        Debug.Log("ActionStateManager: Switching to Gun");
        if (playerSetupView == null)
        {
            Debug.LogError("ActionStateManager: playerSetupView is null when switching to gun!");
        }
        else
        {
            playerSetupView.RPC("SetTPWeapon", RpcTarget.All, 2);
        }

        if (gun == null)
        {
            Debug.LogError("ActionStateManager: gun is null when switching to gun!");
        }
        else
        {
            gun.SetActive(true);
        }

        SetLayerWeight(1, 1);  // Gun layer
    }

    public void OnClick()
    {
        Debug.Log("ActionStateManager: OnClick called");
        lastClickedTime = Time.time;
        noOfClicks++;

        if (anim == null)
        {
            Debug.LogError("ActionStateManager: anim is null in OnClick!");
            return;
        }

        anim.SetBool("SwordAction1", true);
        
        noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);

        if (noOfClicks >= 2 && anim.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(2).IsName("SwordAction1"))
        {
            anim.SetBool("SwordAction1", false);
            anim.SetBool("SwordAction2", true);
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        Debug.Log($"ActionStateManager: TakeDamage called with damage: {damage}");
        if (playerHealth > 0)
        {
            playerHealth -= damage;
            if (playerHealth <= 0)
            {
                photonView.RPC("PlayerDeath", RpcTarget.All, photonView.ViewID);
            }
            else
                Debug.Log("Player Hit!!");
        }
    }

    [PunRPC]
    void PlayerDeath(int viewID)
    {
        Debug.Log($"ActionStateManager: PlayerDeath called with viewID: {viewID}");
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView == null)
        {
            Debug.LogError($"ActionStateManager: PhotonView with ID {viewID} not found!");
            return;
        }

        targetView.gameObject.SetActive(false);
        if (targetView == photonView)
        {
            isDead = true;
            Invoke("RespawnPlayer", 5f);
        }
    }

    public void RespawnPlayer()
    {
        Debug.Log("ActionStateManager: RespawnPlayer called");
        if (photonView.IsMine)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("ActionStateManager: GameManager.Instance is null!");
                return;
            }

            if (GameManager.Instance.playerSpawnPoint == null)
            {
                Debug.LogError("ActionStateManager: GameManager.Instance.playerSpawnPoint is null!");
                return;
            }

            transform.position = GameManager.Instance.playerSpawnPoint.position;
            isDead = false;
            playerHealth = 30;
        }
        photonView.RPC("RPC_RespawnPlayer", RpcTarget.All, photonView.ViewID);
    }

    [PunRPC]
    public void RPC_RespawnPlayer(int viewID)
    {
        Debug.Log($"ActionStateManager: RPC_RespawnPlayer called with viewID: {viewID}");
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            targetView.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"ActionStateManager: PhotonView with ID {viewID} not found in RPC_RespawnPlayer!");
        }
    }

    public void SwitchState(ActionBaseState state)
    {
        Debug.Log($"ActionStateManager: Switching state to {state.GetType().Name}");
        currentState = state;
        currentState.EnterState(this);
    }

    void SetLayerWeight(int layerIndex, float weight)
    {
        Debug.Log($"ActionStateManager: Setting layer {layerIndex} weight to {weight}");
        if (anim == null)
        {
            Debug.LogError("ActionStateManager: anim is null in SetLayerWeight!");
            return;
        }

        for (int i = 0; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }
        anim.SetLayerWeight(layerIndex, weight);
    }

    public void ReloadWeapon()
    {
        Debug.Log("ActionStateManager: ReloadWeapon called");
        if (ammo != null)
            ammo.Reload();
        else
            Debug.LogError("ActionStateManager: ammo is null in ReloadWeapon!");

        SwitchState(Default);
    }

    public void Magout()
    {
        Debug.Log("ActionStateManager: Magout called");
        if (ammo != null && audioSource != null)
            audioSource.PlayOneShot(ammo.magOutSound);
        else
            Debug.LogError("ActionStateManager: ammo or audioSource is null in Magout!");
    }

    public void MagIn()
    {
        Debug.Log("ActionStateManager: MagIn called");
        if (ammo != null && audioSource != null)
            audioSource.PlayOneShot(ammo.magInSound);
        else
            Debug.LogError("ActionStateManager: ammo or audioSource is null in MagIn!");
    }

    public void ReleaseSlide()
    {
        Debug.Log("ActionStateManager: ReleaseSlide called");
        if (ammo != null && audioSource != null)
            audioSource.PlayOneShot(ammo.releaseSlideSound);
        else
            Debug.LogError("ActionStateManager: ammo or audioSource is null in ReleaseSlide!");
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Debug.Log("ActionStateManager: OnDisable called");
        // Clean up any resources if necessary
    }
}