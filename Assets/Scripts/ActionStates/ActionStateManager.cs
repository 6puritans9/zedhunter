using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ActionStateManager : MonoBehaviourPun
{
    [HideInInspector]public ActionBaseState currentState;

    public ReloadState Reload = new ReloadState();
	public DefaultState Default = new DefaultState();
    public SwordAction SwordAction = new SwordAction();

	public bool isDead;
	public int playerHealth = 500;
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

    private Volume postProcessVolume;
    private Vignette vignette; // 비네트 효과
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

		gun.SetActive(false);
		gunRig.weight = 0;
		SetLayerWeight(0, 1);  // ���� ����
		playerSetipView.RPC("SetTPWeapon", RpcTarget.All, 1);

		anim.SetInteger("Health",playerHealth)
	}

    // Update is called once per frame
    void Update()
    {
			// �Է¿� ���� ���̾� ����ġ ����
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				playerSetipView.RPC("SetTPWeapon", RpcTarget.All, 1);
				gun.SetActive(false);
				//gunRig.weight = 0;
				SetLayerWeight(0, 1);  // ���� ����
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				playerSetipView.RPC("SetTPWeapon", RpcTarget.All, 2);
				gun.SetActive(true);
				//gunRig.weight = 1;
				SetLayerWeight(1, 1);  // ��
			}

			anim.SetInteger("Health", playerHealth);
			currentState.UpdateState(this);
			UpdateHealthEffect(); // 체력 효과 업데이트
    }

    void UpdateHealthEffect()
    {
        if (vignette != null)
        {
            float healthPercentage = (float)playerHealth / maxPlayerHealth;
            float vignetteIntensity = Mathf.Lerp(1.5f, 0f, healthPercentage);
            vignette.intensity.Override(vignetteIntensity);
            Debug.Log($"Health: {playerHealth}, Intensity: {vignetteIntensity}");
        }
        else
        {
            Debug.LogError("Vignette effect is null!");
        }
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
				anim.SetInteger("Health", playerHealth);
        UpdateHealthEffect(); // 데미지를 입을 때마다 효과 업데이트
			if (playerHealth <= 0)
				PlayerDeath();
			else
				Debug.Log("Player Hit!!");
        }
    }

	[PunRPC]
	void PlayerDeath()
    {
		Debug.Log("Player Death");
	}



	public void SwitchState(ActionBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
	void SetLayerWeight(int layerIndex, float weight)
	{
		// ��� ���̾��� ����ġ�� 0���� ����
		for (int i = 0; i < anim.layerCount; i++)
		{
			anim.SetLayerWeight(i, 0);
		}
		// ������ ���̾��� ����ġ�� ����
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
