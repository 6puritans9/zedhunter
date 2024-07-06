using Photon.Pun;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.XR;

public class MovementStateManager : MonoBehaviourPun {
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

    [SerializeField] private float gravity = -9.81f;
    Vector3 velocity;

    [SerializeField] private float jumpHeight = 1.0f;

    [HideInInspector] public MovementBaseState currentState;


    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public CrouchState Crouch = new CrouchState();
    public RunState Run = new RunState();

    [HideInInspector] public Animator anim;

    PlayerSetup playerSetup;


    #region Init_and_Update

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        playerSetup = GetComponent<PlayerSetup>();
        //controller = GetComponent<CharacterController>();
        SwitchState(Idle);

        // Cursor settings
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        dir = transform.forward * vInput + transform.right * hzInput;
        dir.y = 0;

        playerSetup.anim.SetFloat("hzInput", hzInput);
        playerSetup.anim.SetFloat("vInput", vInput);

        currentState.UpdateState(this);
    }

    void FixedUpdate() {
        Move();
        ApplyGravity();
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

    bool IsGrounded() {
        spherePos = transform.position + Vector3.down * groundYOffset;
        return Physics.CheckSphere(spherePos, capsuleCollider.radius, groundMask, QueryTriggerInteraction.Ignore);
    }

    void ApplyGravity() {
        if (IsGrounded() && rb.velocity.y < 0) {
            rb.velocity = new Vector3(rb.velocity.x, -2f, rb.velocity.z);
        }
        else {
            rb.AddForce(new Vector3(0, gravity, 0), ForceMode.Acceleration);
        }
    }

    public void Jump() {
        if (IsGrounded()) {
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2 * gravity), ForceMode.Impulse);
        }
    }

    #endregion
}