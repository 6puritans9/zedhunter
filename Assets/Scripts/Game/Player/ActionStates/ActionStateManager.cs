using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ActionStateManager : MonoBehaviour
    {
        [HideInInspector] public ActionBaseState currentState;
        private PlayerSetup playerSetup;

        public DefaultState Default = new DefaultState();
        public ReloadState Reload = new ReloadState();

        public Rig gunRig;
        public GameObject gun;
        public GameObject currentWeapon;
        [HideInInspector] public WeaponAmmo ammo;

        public MultiAimConstraint rHandAim;
        public TwoBoneIKConstraint lHandIK;

        public float cooldownTime = 2f;
        public float nextFireTime = 0f;
        public float lastClickedTime = 0;
        public static int numOfClicks = 0;
        public float maxComboDelay = 1;

        [HideInInspector] public Animator anim;
        AudioSource audioSource;

        private void Awake()
            {
                // Debug.Log("ActionStateManager: Awake called");

                anim = GetComponent<Animator>();
                if (anim == null)
                    {
                        Debug.LogError("ActionStateManager: Animator component not found!");
                    }
            }

        void Start()
            {
                // Debug.Log("ActionStateManager: Start called");

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
                
                playerSetup = GetComponent<PlayerSetup>();
                
                SwitchState(Default);

                gun.SetActive(false);
                gunRig.weight = 0;
                SetLayerWeight(0, 1); // Sword layer

                SwitchToGun();
            }

        void Update()
            {
                if (currentState == null)
                    {
                        Debug.LogError("ActionStateManager: currentState is null!");
                    }
                else
                    {
                        currentState.UpdateState(this);
                    }

                
            }


        // void SwitchToSword()
        //     {
        //         Debug.Log("ActionStateManager: Switching to Sword");
        //         if (playerSetupView == null)
        //             {
        //                 Debug.LogError("ActionStateManager: playerSetupView is null when switching to sword!");
        //             }
        //         else
        //             {
        //                 playerSetupView.RPC("SetTPWeapon", RpcTarget.All, 1);
        //             }
        //
        //         if (gun == null)
        //             {
        //                 Debug.LogError("ActionStateManager: gun is null when switching to sword!");
        //             }
        //         else
        //             {
        //                 gun.SetActive(false);
        //             }
        //
        //         SetLayerWeight(0, 1); // Sword layer
        //     }

        void SwitchToGun()
            {
                Debug.Log("ActionStateManager: Switching to Gun");

                playerSetup.SetTPWeapon(2);
                gun.SetActive(true);
                SetLayerWeight(1, 1); // Gun layer
            }

        public void OnClick()
            {
                Debug.Log("ActionStateManager: OnClick called");
                lastClickedTime = Time.time;
                numOfClicks++;

                if (anim == null)
                    {
                        Debug.LogError("ActionStateManager: anim is null in OnClick!");
                        return;
                    }

                anim.SetBool("SwordAction1", true);

                numOfClicks = Mathf.Clamp(numOfClicks, 0, 3);

                if (numOfClicks >= 2 && anim.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.7f &&
                    anim.GetCurrentAnimatorStateInfo(2).IsName("SwordAction1"))
                    {
                        anim.SetBool("SwordAction1", false);
                        anim.SetBool("SwordAction2", true);
                    }
            }


        // public void TakeDamage(int damage)
        //     {
        //         Debug.Log($"ActionStateManager: TakeDamage called with damage: {damage}");
        //         if (playerHealth > 0)
        //             {
        //                 playerHealth -= damage;
        //                 if (playerHealth <= 0)
        //                     {
        //                         PlayerDeath();
        //                     }
        //                 else
        //                     Debug.Log("Player Hit!!");
        //             }
        //     }
        //
        // void PlayerDeath()
        //     {
        //         // TODO: need fix
        //         gameObject.SetActive(false);
        //
        //         isDead = true;
        //         Invoke("RespawnPlayer", 5f);
        //     }

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
    }