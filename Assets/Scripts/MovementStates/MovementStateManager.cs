using Photon.Pun;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.XR;

public class MovementStateManager : MonoBehaviourPun, IPunObservable {
    public float currentMoveSpeed;
    public float walkSpeed = 3, walkBackSpeed = 2;
    public float runSpeed = 7, runBackSpeed = 5;
    public float crouchSpeed = 2, crouchBackSpeed = 1;

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
    public CrouchState Crouch = new CrouchState();
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

        // Cursor settings
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PhotonNetwork.SerializationRate = 30; // �ʴ� ��Ŷ ���� Ƚ�� ����
        PhotonNetwork.SendRate = 60;          // �ʴ� ��Ʈ��ũ ������Ʈ Ƚ�� ����

        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void Update() {
        if (photonView.IsMine)
        {
            hzInput = Input.GetAxis("Horizontal");
            vInput = Input.GetAxis("Vertical");

            dir = (transform.forward * vInput) + (transform.right * hzInput);
            dir.y = 0;

            anim.SetFloat("hzInput", hzInput);
            anim.SetFloat("vInput", vInput);

            currentState.UpdateState(this);
        }
        else
        {
            float lerpFactor = (Time.time - lastReceivedTime) * PhotonNetwork.SerializationRate;
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpFactor);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpFactor);
        }
    }

    void FixedUpdate() {
        if(photonView.IsMine)
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            //���� ��ġ�� ȸ�� ����
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //��ġ�� ȸ�� �����͸� �����Ͽ� ��ǥ��ġ�� ȸ������ ����
            targetPosition = (Vector3)stream.ReceiveNext();
            targetRotation = (Quaternion)stream.ReceiveNext();
            lastReceivedTime = Time.time;
        }
    }




    #endregion
}