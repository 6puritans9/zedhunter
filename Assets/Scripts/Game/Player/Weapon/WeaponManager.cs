using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class WeaponManager : MonoBehaviour
{
	[Header("Fire Rate")]
	[SerializeField] float fireRate;
	[SerializeField] bool semiAuto;
	float fireRateTimer;


	[Header("Bullet Properties")]
	[SerializeField] GameObject hitParticle;
	[SerializeField] GameObject hitParticleToZombie;
	[SerializeField] Transform barrelPos;
	[SerializeField] float bulletVelocity;
	[SerializeField] int bulletPerShot;
	public float damage = 20;
	AimStateManager aim;


	private int layerMask;


	[SerializeField] AudioClip gunShot;
	[SerializeField] AudioClip bulletShot;
	[SerializeField] AudioClip bulletHeadShot;
	AudioSource bulletShotAudioSource;
	AudioSource bulletHeadShotAudioSource;
	AudioSource gunShotAudioSource;
	WeaponAmmo ammo;
	WeaponBloom bloom;
	ActionStateManager actions;
	WeaponRecoil recoil;


	Light muzzleFlashLight;
	ParticleSystem muzzleFlashParticles;
	float lightIntensity;
	[SerializeField] float lightReturnSpeed = 20;


	public float enemyKickbackForce = 100;


	public Queue<ParticleSystem> hitParticlePool = new Queue<ParticleSystem>();
	public Queue<ParticleSystem> hitParticleToZombiePool = new Queue<ParticleSystem>();
	[SerializeField] private int initialParticlePoolSize = 5;


	void Start()
	{
		layerMask = ~((1 << 7) | (1 << 11));
		recoil = GetComponent<WeaponRecoil>();
		aim = GetComponentInParent<AimStateManager>();
		ammo = GetComponent<WeaponAmmo>();
		bloom = GetComponent<WeaponBloom>();
		actions = GetComponentInParent<ActionStateManager>();
		muzzleFlashLight = GetComponentInChildren<Light>();
		lightIntensity = muzzleFlashLight.intensity;
		muzzleFlashLight.intensity = 0;
		muzzleFlashParticles = GetComponentInChildren<ParticleSystem>();
		fireRateTimer = fireRate;

		gunShotAudioSource = GetComponent<AudioSource>();
		gunShotAudioSource.clip = gunShot;
		gunShotAudioSource.volume = 0.2f;

		bulletShotAudioSource = gameObject.AddComponent<AudioSource>();
		bulletShotAudioSource.clip = bulletShot;
		bulletShotAudioSource.volume = 1f;

		bulletHeadShotAudioSource = gameObject.AddComponent<AudioSource>();
		bulletHeadShotAudioSource.clip = bulletHeadShot;
		bulletHeadShotAudioSource.volume = 1;

		InitializeHitParticlePool();
		InitializeHitParticleToZombiePool();
	}


	void Update()
	{
		if (ShouldFire())
		{
			Fire();
		}
			muzzleFlashLight.intensity = Mathf.Lerp(muzzleFlashLight.intensity, 0, lightReturnSpeed * Time.deltaTime);
	}


	void InitializeHitParticlePool()
	{
		for (int i = 0; i < initialParticlePoolSize; i++)
		{
			GameObject particleObject = Instantiate(hitParticle, Vector3.zero, Quaternion.identity);
			ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
			particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			particleObject.SetActive(false);
			hitParticlePool.Enqueue(particleSystem);
		}
	}

	void InitializeHitParticleToZombiePool()
	{
		for (int i = 0; i < initialParticlePoolSize; i++)
		{
			GameObject particleObject = Instantiate(hitParticleToZombie, Vector3.zero, Quaternion.identity);
			ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
			particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			particleObject.SetActive(false);
			hitParticleToZombiePool.Enqueue(particleSystem);
		}
	}


	void GetHitParticle(Vector3 position, Quaternion rotation)
	{
		ParticleSystem particleSystem;


		if (hitParticlePool.Count > 0)
		{
			particleSystem = hitParticlePool.Dequeue();
		}
		else
		{
			GameObject particleObject = Instantiate(hitParticle, position, rotation);
			particleSystem = particleObject.GetComponent<ParticleSystem>();
		}

		if (particleSystem != null)
		{
			particleSystem.transform.position = position;
			particleSystem.transform.rotation = rotation;
			particleSystem.gameObject.SetActive(true);
			particleSystem.Play();


			//if(photonView.IsMine)
			// 파티클 시스템 큐에 추가
			AddToHitParticlePool(particleSystem);
			StartCoroutine(DeactivateParticleSystem(particleSystem));
		}

	}

	void GetHitParticleToZombie(Vector3 position, Quaternion rotation)
	{
		ParticleSystem particleSystem;


		if (hitParticlePool.Count > 0)
		{
			particleSystem = hitParticleToZombiePool.Dequeue();
		}
		else
		{
			GameObject particleObject = Instantiate(hitParticleToZombie, position, rotation);
			particleSystem = particleObject.GetComponent<ParticleSystem>();
		}

		if (particleSystem != null)
		{
			particleSystem.transform.position = position;
			particleSystem.transform.rotation = rotation;
			particleSystem.gameObject.SetActive(true);
			particleSystem.Play();


			//if(photonView.IsMine)
			// 파티클 시스템 큐에 추가
			AddToHitParticleToZombiePool(particleSystem);
			StartCoroutine(DeactivateParticleToZombieSystem(particleSystem));
		}

	}


	void AddToHitParticlePool(ParticleSystem particleSystem)
	{
		if (particleSystem != null)
		{
			hitParticlePool.Enqueue(particleSystem);
		}
	}

	void AddToHitParticleToZombiePool(ParticleSystem particleSystem)
	{
		if (particleSystem != null)
		{
			hitParticleToZombiePool.Enqueue(particleSystem);
		}
	}


	IEnumerator DeactivateParticleSystem(ParticleSystem particleSystem)
	{
		yield return new WaitForSeconds(particleSystem.main.duration);
		if(particleSystem)
		{
			particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			particleSystem.gameObject.SetActive(false);
			hitParticlePool.Enqueue(particleSystem);
		}
	}

	IEnumerator DeactivateParticleToZombieSystem(ParticleSystem particleSystem)
	{
		yield return new WaitForSeconds(particleSystem.main.duration);
		if(particleSystem)
		{
			particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			particleSystem.gameObject.SetActive(false);
			hitParticleToZombiePool.Enqueue(particleSystem);
		}
	}


	bool ShouldFire()
	{
		fireRateTimer += Time.deltaTime;
		if (fireRateTimer < fireRate) return false;
		if (ammo.currentAmmo == 0) return false;
		if (actions.currentState == actions.Reload) return false;
		if (semiAuto && Input.GetKeyDown(KeyCode.Mouse0)) return true;
		if (!semiAuto && Input.GetKey(KeyCode.Mouse0)) return true;
		return false;
	}


	void Fire()
	{
		fireRateTimer = 0;


		barrelPos.LookAt(aim.aimPos);
		barrelPos.localEulerAngles = bloom.BloomAngle(barrelPos);


		gunShotAudioSource.PlayOneShot(gunShot);
		recoil.TriggerRecoil();
		TriggerMuzzleFlash();
		ammo.currentAmmo--;
		for (int i = 0; i < bulletPerShot; i++)
		{
			ShootRay();
		}
	}


	void ShootRay()
	{
		Ray ray = new Ray(barrelPos.position, barrelPos.forward);
		RaycastHit hit;


		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			if (!hit.collider.gameObject.CompareTag("Enemy"))
				GetHitParticle(hit.point, Quaternion.LookRotation(hit.normal));
			else
			{
				if (hit.collider.gameObject.name == "HeadShot")
				{
					bulletHeadShotAudioSource.PlayOneShot(bulletHeadShotAudioSource.clip);
					ChangeCrosshairSprite.Instance.ChangeSprite(1);
				}
				else bulletShotAudioSource.Play();

				GetHitParticleToZombie(hit.point, Quaternion.LookRotation(hit.normal));
			}
			
			if (hit.collider.TryGetComponent(out WallHP wallHP))
			{
				wallHP.TakeDamage((int)damage);
			}

			//몸통샷
			if (hit.collider.TryGetComponent(out EnemyHealth enemyHealth))
			{
				enemyHealth.TakeDamage(damage);


				if (enemyHealth.health <= 0 && !enemyHealth.isDead)
					enemyHealth.isDead = true;
			}
			//헤드샷
			else if (hit.collider.TryGetComponent(out EnemyHeadShotDamage enemyHeadShotDamage))
			{
				enemyHeadShotDamage.TakeHeadShotDamage(damage * 2f);


				if (enemyHeadShotDamage.enemyHealth.health <= 0 && !enemyHeadShotDamage.enemyHealth.isDead)
					enemyHeadShotDamage.enemyHealth.isDead = true;
			}
		}
	}


	void TriggerMuzzleFlash()
	{
		muzzleFlashParticles.Play();
		muzzleFlashLight.intensity = lightIntensity;
	}
}