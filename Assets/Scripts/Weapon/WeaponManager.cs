using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviourPun
{
    public PhotonView photonView;

    [Header("Fire Rate")]
    [SerializeField] float fireRate;
    [SerializeField] bool semiAuto;
    float fireRateTimer;

    [Header("Bullet Properties")]
    //[SerializeField] GameObject bullet;
    [SerializeField] GameObject hitParticle;
    [SerializeField] Transform barrelPos;
    [SerializeField] float bulletVelocity;
    [SerializeField] int bulletPerShot;
    public float damage = 20;
    AimStateManager aim;

    private int layerMask;

    [SerializeField] AudioClip gunShot;
    AudioSource audioSource;
    WeaponAmmo ammo;
    WeaponBloom bloom;
    ActionStateManager actions;
    WeaponRecoil recoil;

    Light muzzleFlashLight;
    ParticleSystem muzzleFlashParticles;
    float lightIntensity;
    [SerializeField] float lightReturnSpeed = 20;

    public float enemyKickbackForce = 100;

    Queue<ParticleSystem> hitParticlePool = new Queue<ParticleSystem>();
    [SerializeField] private int iniialParticlePoolSize = 10;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //7번(Player)레이어 제외 
        layerMask = ~((1 << 7) | (1 << 11));
        recoil = GetComponent<WeaponRecoil>();
        audioSource = GetComponent<AudioSource>();
        aim = GetComponentInParent<AimStateManager>();
        ammo = GetComponent<WeaponAmmo>();
        bloom = GetComponent<WeaponBloom>();
        actions = GetComponentInParent<ActionStateManager>();
        muzzleFlashLight = GetComponentInChildren<Light>();
        lightIntensity = muzzleFlashLight.intensity;
        muzzleFlashLight.intensity = 0;
        muzzleFlashParticles = GetComponentInChildren<ParticleSystem>();
        fireRateTimer = fireRate;


        //InitializeHitParticlePool();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (ShouldFire())
            {
                Fire();
                //photonView.RPC("Fire", RpcTarget.All);
            }
            muzzleFlashLight.intensity = Mathf.Lerp(muzzleFlashLight.intensity,
                0, lightReturnSpeed * Time.deltaTime);
        }
    }
    //파티클 10개 초기화
    void InitializeHitParticlePool()
    {
        for (int i = 0; i < iniialParticlePoolSize; i++)
        {
            GameObject particleObject = PhotonNetwork.Instantiate(hitParticle.name, Vector3.zero, Quaternion.identity);
            ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleObject.SetActive(false);
            hitParticlePool.Enqueue(particleSystem);
        }
    }
    //Pool에 있는 파티클 꺼내오기
    [PunRPC]
    ParticleSystem GetHitParticle(Vector3 position, Quaternion rotation)
    {

        ParticleSystem particleSystem;
        if (hitParticlePool.Count > 0)
        {
            particleSystem = hitParticlePool.Dequeue();
        }
        else
        {
            GameObject particleObject = PhotonNetwork.Instantiate(hitParticle.name, position, rotation);
            particleSystem = particleObject.GetComponent<ParticleSystem>();
        }

        if (particleSystem != null)
        {
            particleSystem.transform.position = position;
            particleSystem.transform.rotation = rotation;
            particleSystem.gameObject.SetActive(true);
            particleSystem.Play();

            //StartCoroutine(DeactivateParticleSystem(particleSystem));
            // 파티클 비활성화 및 풀로 반환하는 작업을 별도의 메서드로 처리
            if (photonView.IsMine)
                StartCoroutine(DeactivateParticleSystem(particleSystem));
        }
        return particleSystem;
    }

    //파티클 비활성화 후 Pool에 넣기
    IEnumerator DeactivateParticleSystem(ParticleSystem particleSystem)
    {
        yield return new WaitForSeconds(particleSystem.main.duration);
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particleSystem.gameObject.SetActive(false);
        hitParticlePool.Enqueue(particleSystem);
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

    [PunRPC]
    void Fire()
    {
        fireRateTimer = 0;

        barrelPos.LookAt(aim.aimPos);
        barrelPos.localEulerAngles = bloom.BloomAngle(barrelPos);

        audioSource.PlayOneShot(gunShot);
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
            //맞은 지점 파티클 생성
            //ParticleSystem hitEffect = GetHitParticle(hit.point, Quaternion.LookRotation(hit.normal));
            photonView.RPC("GetHitParticle", RpcTarget.All, hit.point, Quaternion.LookRotation(hit.normal));

            if (hit.collider.TryGetComponent(out EnemyHealth enemyHealth))
            {
                PhotonView enemyPhotonView = enemyHealth.GetComponent<PhotonView>();
                enemyPhotonView.RPC("TakeDamage", RpcTarget.All, damage);

                if (enemyHealth.health <= 0 && !enemyHealth.isDead)
                    enemyHealth.isDead = true;
            }
        }
    }

    void TriggerMuzzleFlash()
    {
        muzzleFlashParticles.Play();
        muzzleFlashLight.intensity = lightIntensity;
    }
}