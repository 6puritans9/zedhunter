using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
    {
        [Header("Fire Rate")] [SerializeField] bool semiAuto;
        [SerializeField] float fireRate;
        float fireRateTimer;

        [Header("Bullet Properties")] [SerializeField]
        private AimStateManager aim;

        GameObject hitParticle;
        [SerializeField] Transform barrelPos;
        [SerializeField] float bulletVelocity;
        [SerializeField] int bulletPerShot;
        public float damage = 20;

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

        public Queue<ParticleSystem> hitParticlePool = new Queue<ParticleSystem>();
        [SerializeField] private int initialParticlePoolSize = 10;

        // private void Awake()
        //     {
        //         layerMask = ~((1 << 7) | (1 << 11));
        //         actions = GetComponent<ActionStateManager>();
        //         if (actions == null) Debug.LogError("ActionStateManager not found!");
        //
        //         aim = GetComponent<AimStateManager>();
        //         recoil = GetComponentInChildren<WeaponRecoil>();
        //         audioSource = GetComponentInChildren<AudioSource>();
        //         ammo = GetComponentInChildren<WeaponAmmo>();
        //         if (ammo == null) Debug.LogError("WeaponAmmo not found in children!");
        //
        //         bloom = GetComponentInChildren<WeaponBloom>();
        //         muzzleFlashLight = GetComponentInChildren<Light>();
        //         lightIntensity = muzzleFlashLight.intensity;
        //         
        //         muzzleFlashParticles = GetComponentInChildren<ParticleSystem>();
        //         muzzleFlashLight.intensity = 0;
        //         
        //         fireRateTimer = fireRate;
        //     }

        void Start()
            {
                // layerMask = ~((1 << 7) | (1 << 11));
                // actions = GetComponent<ActionStateManager>();
                // if (actions == null) Debug.LogError("ActionStateManager not found!");
                //
                // aim = GetComponent<AimStateManager>();
                // recoil = GetComponentInChildren<WeaponRecoil>();
                // audioSource = GetComponentInChildren<AudioSource>();
                // ammo = GetComponentInChildren<WeaponAmmo>();
                // if (ammo == null) Debug.LogError("WeaponAmmo not found in children!");
                //
                // bloom = GetComponentInChildren<WeaponBloom>();
                // muzzleFlashLight = GetComponentInChildren<Light>();
                // lightIntensity = muzzleFlashLight.intensity;
                // muzzleFlashParticles = GetComponentInChildren<ParticleSystem>();
                // muzzleFlashLight.intensity = 0;
                // fireRateTimer = fireRate;

                layerMask = ~((1 << 7) | (1 << 11));
                actions = GetComponent<ActionStateManager>();
                if (actions == null) Debug.LogError("ActionStateManager not found!");

                aim = GetComponent<AimStateManager>();
                recoil = GetComponentInChildren<WeaponRecoil>();
                audioSource = GetComponentInChildren<AudioSource>();
                ammo = GetComponentInChildren<WeaponAmmo>();
                if (ammo == null) Debug.LogError("WeaponAmmo not found in children!");

                bloom = GetComponentInChildren<WeaponBloom>();
                muzzleFlashLight = GetComponentInChildren<Light>();
                lightIntensity = muzzleFlashLight.intensity;

                muzzleFlashParticles = GetComponentInChildren<ParticleSystem>();
                muzzleFlashLight.intensity = 0;

                fireRateTimer = fireRate;

                InitializeHitParticlePool();
            }


        void Update()
            {
                if (ShouldFire())
                    {
                        Fire();
                    }

                muzzleFlashLight.intensity = Mathf.Lerp(muzzleFlashLight.intensity, 0,
                    lightReturnSpeed * Time.deltaTime);
            }

        void InitializeHitParticlePool()
            {
                for (int i = 0; i < initialParticlePoolSize; i++)
                    {
                        GameObject particleObject =
                            Instantiate(hitParticle, Vector3.zero, Quaternion.identity);
                        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
                        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                        particleObject.SetActive(false);
                        hitParticlePool.Enqueue(particleSystem);
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
                        AddToHitParticlePool();
                        StartCoroutine(DeactivateParticleSystem(particleSystem));
                    }
            }


        void AddToHitParticlePool()
            {
                ParticleSystem particleSystem = GetComponent<ParticleSystem>();
                if (particleSystem != null)
                    {
                        hitParticlePool.Enqueue(particleSystem);
                    }
            }


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
                        GetHitParticle(hit.point, Quaternion.LookRotation(hit.normal));
                    }


                // if (hit.collider.TryGetComponent(out EnemyHealth enemyHealth))
                {
                    // PhotonView enemyPhotonView = enemyHealth.GetComponent<PhotonView>();
                    // enemyPhotonView.RPC("TakeDamage", RpcTarget.All, damage);
                    //
                    //
                    // if (enemyHealth.health <= 0 && !enemyHealth.isDead)
                    //     enemyHealth.isDead = true;
                }
            }

        private void TriggerMuzzleFlash()
            {
                muzzleFlashParticles.Play();
                muzzleFlashLight.intensity = lightIntensity;
            }
    }