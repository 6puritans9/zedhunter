using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ActionStateManager : MonoBehaviour
{
	[HideInInspector] public ActionBaseState currentState;

	public ReloadState Reload = new ReloadState();
	public DefaultState Default = new DefaultState();

	public bool isDead;
	int playerMaxHP = 100;
	public int playerHealth = 100;
	public GameObject currentWeapon;
	[HideInInspector] public WeaponAmmo ammo;
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


	private Volume postProcessVolume;
	private Vignette vignette; // ���Ʈ ȿ��
	public int maxPlayerHealth = 500;

	private void Awake()
	{

		ammo = currentWeapon.GetComponent<WeaponAmmo>();
		anim = GetComponent<Animator>();
		audioSource = currentWeapon.GetComponent<AudioSource>();

		postProcessVolume = FindObjectOfType<Volume>();
		if (postProcessVolume != null && postProcessVolume.profile.TryGet(out vignette))
		{
			vignette.active = true;
			vignette.color.Override(Color.red);
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		SwitchState(Default);


		isDead = false;

		SetWeapon(2);
	}

	// Update is called once per frame
	void Update()
	{

		// �Է¿� ���� ���̾� ����ġ ����
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SetWeapon(1);

		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SetWeapon(2);
		}

		currentState.UpdateState(this);
		UpdateHealthEffect(); // ü�� ȿ�� ������Ʈ
	}

	void UpdateHealthEffect()
	{
		if (vignette != null)
		{
			float healthPercentage = (float)playerHealth / maxPlayerHealth;
			float vignetteIntensity = Mathf.Lerp(1.5f, 0f, healthPercentage);
			vignette.intensity.Override(vignetteIntensity);
		}
		else
		{
			Debug.LogError("Vignette effect is null!");
		}
	}

	void SetWeapon(int idx)
	{
		switch (idx)
		{
			case 1:
				gun.SetActive(false);
				//gunRig.weight = 0;
				SetLayerWeight(0, 1);
				break;
			case 2:
				gun.SetActive(true);
				//gunRig.weight = 1;
				SetLayerWeight(1, 1);
				break;
		}
	}


	public void TakeDamage(int damage)
	{
		if (playerHealth > 0)
		{
			playerHealth -= damage;
			if (playerHealth <= 0)
			{
				PlayerDeath();
			}
			else
				Debug.Log("Player Hit!!");
		}
	}

	void PlayerDeath()
	{
		isDead = true;
		Invoke("RespawnPlayer", 5f); // 5�� �Ŀ� �����
	}

	void RPC_PlayerDeath()
	{
		gameObject.SetActive(false);
	}

	public void RespawnPlayer()
	{
		isDead = false;
		playerHealth = playerMaxHP;
		RPC_RespawnPlayer();
	}

	public void RPC_RespawnPlayer()
	{
		gameObject.SetActive(true);
	}


	public void SwitchState(ActionBaseState state)
	{
		currentState = state;
		currentState.EnterState(this);
	}

	void SetLayerWeight(int layerIndex, int weight)
	{
		// ��� ���̾��� ����ġ�� 0���� ����
		for (int i = 0; i < anim.layerCount; i++)
		{
			anim.SetLayerWeight(i, 0);
		}

		if (layerIndex == 1 && weight > 0.9f)
			gunRig.weight = 1;
		else
			gunRig.weight = 0;

		// ������ ���̾��� ����ġ�� ����
		anim.SetLayerWeight(layerIndex, weight);
	}

	public void ReloadWeapon()
	{
		if (ammo != null)
			ammo.Reload();
		SwitchState(Default);
	}

	public void Magout()
	{
		if (ammo != null && audioSource != null)
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
