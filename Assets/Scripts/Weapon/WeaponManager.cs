using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
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
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldFire()) Fire();
        muzzleFlashLight.intensity = Mathf.Lerp(muzzleFlashLight.intensity, 
            0, lightReturnSpeed * Time.deltaTime);
    }

    bool ShouldFire()
    {
        fireRateTimer += Time.deltaTime;
        if (fireRateTimer < fireRate) return false;
        if(ammo.currentAmmo == 0) return false;
        if(actions.currentState == actions.Reload) return false;
        if (semiAuto && Input.GetKeyDown(KeyCode.Mouse0)) return true;
		if (!semiAuto && Input.GetKey(KeyCode.Mouse0)) return true;
        return false;
    }

    void Fire()
    {
        fireRateTimer = 0;

        barrelPos.LookAt(aim.aimPos);
        barrelPos.localEulerAngles = bloom.BloomAngle(barrelPos);

        audioSource.PlayOneShot(gunShot);
        recoil.TriggerRecoil();
        TriggerMuzzleFlash();
        ammo.currentAmmo--;
        for(int i = 0; i < bulletPerShot; i++)
        {
            ShootRay();
        }
    }

    void ShootRay()
    {
        Ray ray = new Ray(barrelPos.position, barrelPos.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            //맞은 지점 파티클 생성
            GameObject hitEffect = PhotonNetwork.Instantiate(hitParticle.name, hit.point, Quaternion.LookRotation(hit.normal));
            //PhotonNetwork.Destroy(hitEffect);

            if(hit.collider.TryGetComponent(out EnemyHealth enemyHealth))
            {
                PhotonView enemyPhotonView = enemyHealth.GetComponent<PhotonView>();
                enemyPhotonView.RPC("TakeDamage", RpcTarget.All, damage);

                if(enemyHealth.health <= 0 && !enemyHealth.isDead)
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
