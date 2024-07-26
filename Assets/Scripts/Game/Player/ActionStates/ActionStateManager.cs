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
	public WallBuilder WallBuilder;

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

	private bool isGameOver = false;

	private void Awake()
	{
		WallBuilder = GetComponent<WallBuilder>();
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
			float vignetteIntensity = Mathf.Lerp(0.5f, 0f, healthPercentage);
			vignette.intensity.Override(vignetteIntensity);
		}
		else
		{
			Debug.LogError("Vignette effect is null!");
		}
	}

	public void SetWeapon(int idx)
	{
		switch (idx)
		{
			case 1:
				//gunRig.weight = 0;
				SetLayerWeight(0, 1);
				break;
			case 2:
				//gunRig.weight = 1;
				SetLayerWeight(1, 1);
				break;
		}
	}


	public void TakeDamage(int damage)
	{
		if (playerHealth > 0)
		{
			if (this.gameObject.name == "Kafka(Clone)" || this.gameObject.name == "Serval(Clone)")
				PlayerSoundSource.Instance.GetTakeDamageServalKafkaSound();
			else if (this.gameObject.name == "Yanqing(Clone)")
				PlayerSoundSource.Instance.GetTakeDamageYanqingSound();

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
		// Invoke("RespawnPlayer", 5f); // 5    Ŀ       
		if (!isGameOver)
		{
			isGameOver = true;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			SceneTransitionManager.Instance.DissolveToScene("GameOverScene");
		}
	}

	/*void RPC_PlayerDeath()
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
	}*/


	public void SwitchState(ActionBaseState state)
	{
		currentState = state;
		currentState.EnterState(this);
	}

	public void SetLayerWeight(int layerIndex, int weight)
	{
		for (int i = 0; i < anim.layerCount; i++)
		{
			anim.SetLayerWeight(i, 0);
		}

		if (layerIndex == 1)
		{
			gunRig.weight = 1;
			gun.SetActive(true);
		}
		else
		{
			gunRig.weight = 0;
			gun.SetActive(false);
		}
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
