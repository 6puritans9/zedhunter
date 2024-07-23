using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.XR;

public class MovementStateManager : MonoBehaviour
    {
    public float currentMoveSpeed;
    public float walkSpeed = 3, walkBackSpeed = 2;
    public float runSpeed = 7, runBackSpeed = 5;

    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float hzInput, vInput;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    [SerializeField] private float groundYOffset;
    [SerializeField] private LayerMask groundMask;
    Vector3 spherePos;
    
    [SerializeField] private float jumpHeight = 1.0f;

    [HideInInspector] public MovementBaseState currentState;
    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public RunState Run = new RunState();

    [HideInInspector] public Animator anim;

    Vector3 targetPosition;
    Quaternion targetRotation;

    float moveSmoothTime = 0.1f;
    float rotationSmoothTime = 0.1f;
    float lastReceivedTime;

    #region Init_and_Update

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        SwitchState(Idle);

        
        
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void Update() {
        
            hzInput = Input.GetAxis("Horizontal");
            vInput = Input.GetAxis("Vertical");

            dir = (transform.forward * vInput) + (transform.right * hzInput);
            dir.y = 0;

            anim.SetFloat("hzInput", hzInput);
            anim.SetFloat("vInput", vInput);
            
            if(hzInput <= 0.1f || vInput <= 0.1f)
            {
			    rb.velocity = Vector3.zero;
			    rb.angularVelocity = Vector3.zero;
		    }

            currentState.UpdateState(this);
        
        // else
        // {
        //     float lerpFactor = (Time.time - lastReceivedTime) * PhotonNetwork.SerializationRate;
        //     // 현재 위치를 보간합니다.
        //     transform.position = Vector3.Lerp(transform.position, targetPosition, lerpFactor);
        //     // 현재 회전을 보간합니다.
        //     transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpFactor);
        // }
    }

    void FixedUpdate()
        {
            Move();
        }

	

	#endregion

	#region Other_Functions

	public void SwitchState(MovementBaseState state) {
        currentState = state;
        currentState.EnterState(this);
    }

    void Move() {
        Vector3 moveVelocity = dir.normalized * currentMoveSpeed;
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if(stream.IsWriting)
    //     {
    //         // 현재 위치와 회전 정보를 보냅니다.
    //         stream.SendNext(transform.position);
    //         stream.SendNext(transform.rotation);
    //     }
    //     else
    //     {
    //         // 위치와 회전 데이터를 수신하여 목표 위치와 회전을 설정합니다.
    //         targetPosition = (Vector3)stream.ReceiveNext();
    //         targetRotation = (Quaternion)stream.ReceiveNext();
    //         lastReceivedTime = Time.time;
    //     }
    // }

    #endregion
}