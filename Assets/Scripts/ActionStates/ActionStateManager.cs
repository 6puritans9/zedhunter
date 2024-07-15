using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionStateManager : MonoBehaviourPun
{
    [HideInInspector]public ActionBaseState currentState;

    public ReloadState Reload = new ReloadState();
	public DefaultState Default = new DefaultState();
    public SwordAction SwordAction = new SwordAction();

	public bool isDead;
	int playerMaxHP = 100;
	public int playerHealth = 100;
	public GameObject currentWeapon;
    [HideInInspector]public WeaponAmmo ammo;
    AudioSource audioSource;

    [HideInInspector] public Animator anim;

    public MultiAimConstraint rHandAim;
    public TwoBoneIKConstraint lHandIK;

	//public GameObject sword;
	public GameObject gun;
    public Rig gunRig;

	//SwordAction
	public float cooldownTime = 2f;
	public float nextFireTime = 0f;
	public static int noOfClicks = 0;
	public float lastClickedTime = 0;
	public float maxComboDelay = 1;
	//SwordAction

	public PhotonView playerSetipView;


    private void Awake()
    {
		ammo = currentWeapon.GetComponent<WeaponAmmo>();
		anim = GetComponent<Animator>();
		audioSource = currentWeapon.GetComponent<AudioSource>();
	}

    // Start is called before the first frame update
    void Start()
    {
		SwitchState(Default);
       

		isDead = false;

		gun.SetActive(false);
		gunRig.weight = 0;
		SetLayerWeight(0, 1);  // 무기 없음
		playerSetipView.RPC("SetTPWeapon", RpcTarget.All, 1);
	}

    // Update is called once per frame
    void Update()
    {

		// 입력에 따라 레이어 가중치 조절
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			playerSetipView.RPC("SetTPWeapon", RpcTarget.All, 1);
			gun.SetActive(false);
			//gunRig.weight = 0;
			SetLayerWeight(0, 1);  // 무기 없음
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			playerSetipView.RPC("SetTPWeapon", RpcTarget.All, 2);
			gun.SetActive(true);
			//gunRig.weight = 1;
			SetLayerWeight(1, 1);  // 총
		}


		currentState.UpdateState(this);
    }

	public void OnClick()
	{
		lastClickedTime = Time.time;
		noOfClicks++;
		anim.SetBool("SwordAction1", true);
		/*if (noOfClicks == 1)
		{
			anim.SetBool("SwordAction1", true);
		}*/
		
		noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);

		if (noOfClicks >= 2
			&& anim.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.7f
			&& anim.GetCurrentAnimatorStateInfo(2).IsName("SwordAction1"))
		{
			anim.SetBool("SwordAction1", false);
			anim.SetBool("SwordAction2", true);
		}
	}



	[PunRPC]
	public void TakeDamage(int damage)
    {
		if(playerHealth > 0)
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
		PhotonView targetView = PhotonView.Find(viewID);
			targetView.gameObject.SetActive(false);
		if (targetView == photonView.IsMine)
		{
			isDead = true;
			Invoke("RespawnPlayer", 5f); // 5초 후에 재생성
		}
	}

	[PunRPC]
	void RPC_PlayerDeath(int viewID)
    {
		PhotonView targetView = PhotonView.Find(viewID);
		if(targetView != null)
        {
			targetView.gameObject.SetActive(false);
		}
    }

	public void RespawnPlayer()
    {
		if(photonView.IsMine)
        {
			transform.position = RoomManager.Instance.playerSpawnPoint.position;
			isDead = false;
			playerHealth = playerMaxHP;
        }
		photonView.RPC("RPC_RespawnPlayer", RpcTarget.All, photonView.ViewID);
	}

	[PunRPC]
	public void RPC_RespawnPlayer(int viewID)
    {
		PhotonView targetView = PhotonView.Find(viewID);
		if (targetView != null)
		{
			targetView.gameObject.SetActive(true);
		}
	}


	public void SwitchState(ActionBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
	void SetLayerWeight(int layerIndex, float weight)
	{
		// 모든 레이어의 가중치를 0으로 설정
		for (int i = 0; i < anim.layerCount; i++)
		{
			anim.SetLayerWeight(i, 0);
		}
		// 지정된 레이어의 가중치를 설정
		anim.SetLayerWeight(layerIndex, weight);
	}

	public void ReloadWeapon()
    {
		if(ammo != null)
			ammo.Reload();
        SwitchState(Default);
    }

    public void Magout()
    {
		if(ammo != null && audioSource != null)
			audioSource.PlayOneShot(ammo.magOutSound);
	}

    public void MagIn()
    {
		if (ammo != null && audioSource != null)
			audioSource.PlayOneShot(ammo.magInSound);
	}

    public void ReleaseSlide()
    {
		if (ammo != null && audioSource != null)
			audioSource.PlayOneShot(ammo.releaseSlideSound);
	}
}
