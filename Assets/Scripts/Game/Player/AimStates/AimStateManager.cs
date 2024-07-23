using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AimStateManager : MonoBehaviour
    {
        [HideInInspector] public AimBaseState currentState;
        public HipFireState Hip = new HipFireState();
        public AimState Aim = new AimState();

        [SerializeField] float mouseSense = 1;
        [SerializeField] Transform camFollowPos;
        float xAxis, yAxis;

        [HideInInspector] public Animator anim;
        [HideInInspector] public CinemachineVirtualCamera vCam;
        public float adsFov = 40;
        [HideInInspector] public float hipFov;
        [HideInInspector] public float currentFov;
        public float fovSmoothSpeed = 10;

        public Transform aimPos;
        [SerializeField] public Vector3 actualAimPos;
        [SerializeField] float aimSmoothSpeed = 20;
        [SerializeField] LayerMask aimMask;

        float xFollowPos;
        float yFollowPos, ogYPos;
        [SerializeField] float crouchCamHeight = 0.03f;
        [SerializeField] float shoulderSwapSpeed = 10;
        MovementStateManager moving;

        // ADS Animation Fix
        private AdsAnimationManager _adsManager;

        private void Awake()
            {
                vCam = GetComponentInChildren<CinemachineVirtualCamera>();
                _adsManager = GetComponent<AdsAnimationManager>();
            }

        // Start is called before the first frame update
        void Start()
            {
                moving = GetComponent<MovementStateManager>();
                xFollowPos = camFollowPos.localPosition.x;
                ogYPos = camFollowPos.localPosition.y;
                yFollowPos = ogYPos;

                hipFov = vCam.m_Lens.FieldOfView;

                anim = GetComponent<Animator>();
                SwitchState(Hip);
            }

        // Update is called once per frame
        void Update()
            {
                xAxis += Input.GetAxisRaw("Mouse X") * mouseSense;
                yAxis -= Input.GetAxisRaw("Mouse Y") * mouseSense;
                yAxis = Mathf.Clamp(yAxis, -80, 80);

                if (vCam.gameObject.activeSelf)
                    vCam.m_Lens.FieldOfView =
                        Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);

                Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
                Ray ray = Camera.main.ScreenPointToRay(screenCenter);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
                    {
                        aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSmoothSpeed * Time.deltaTime);
                    }

                // MoveCamera();

                currentState.UpdateStare(this);

                #region ADS_Animation_Fix

                if (this.currentState == this.Aim)
                    _adsManager.SetValues();

                else
                    _adsManager.ResetValues();

                #endregion
            }

        private void LateUpdate()
            {
                camFollowPos.localEulerAngles = new Vector3(yAxis, camFollowPos.localEulerAngles.y,
                    camFollowPos.localEulerAngles.z);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
            }

        public void SwitchState(AimBaseState state)
            {
                currentState = state;
                currentState.EnterState(this);
            }

        // void MoveCamera()
        //     {
        //         if (Input.GetKeyDown(KeyCode.LeftAlt)) xFollowPos = -xFollowPos;
        //         if (moving.currentState == moving.Crouch) yFollowPos = ogYPos - 0.6f;
        //         else yFollowPos = ogYPos;
        //
        //         Vector3 newFollowPos = new Vector3(xFollowPos, yFollowPos, camFollowPos.localPosition.z);
        //         camFollowPos.localPosition = Vector3.Lerp(camFollowPos.localPosition, newFollowPos,
        //             shoulderSwapSpeed * Time.deltaTime);
        //     }
    }