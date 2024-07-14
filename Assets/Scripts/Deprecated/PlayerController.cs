using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("References")] private CharacterController _controller;
    public Transform camera;
    private Animator _animator;
    

    [Header("Movement Settings")] public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float sprintTransitSpeed = 5f; 
    public float turningSpeed = 2f;
    private float gravity = 9.81f;
    
    private float verticalVelocity;
    private float speed;


    [Header("Input")] private float turnInput;
    private float moveInput;

    private void Start() {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        InputManagement();
        Movement();
        _animator.SetFloat("InputX", turnInput);
        _animator.SetFloat("InputY", moveInput);
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Cursor.lockState == CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void Movement() {
        GroundMovement();
        Turn();
    }

    private void GroundMovement() {
        Vector3 move = new Vector3(turnInput, 0, moveInput);
        move = transform.TransformDirection(move);

        if (Input.GetKey(KeyCode.LeftShift)) {
            speed = Mathf.Lerp(speed, sprintSpeed, sprintTransitSpeed * Time.deltaTime);
        }
        else {
            speed = Mathf.Lerp(speed, walkSpeed, sprintTransitSpeed * Time.deltaTime);
        }
        
        move *= speed;
        move.y = VerticalForceCalculation();
        
        _controller.Move(move * Time.deltaTime);
    }

    private void Turn() {
        if (Mathf.Abs(turnInput) > 0 || Mathf.Abs(moveInput) > 0) {
            Vector3 currentLookDirection = camera.forward;
            currentLookDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(currentLookDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turningSpeed);
        }
    }

    private float VerticalForceCalculation() {
        if (_controller.isGrounded) {
            verticalVelocity = -1f;
        }
        else {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        return verticalVelocity;
    }

    private void InputManagement() {
        turnInput = Input.GetAxis("Horizontal");
        moveInput = Input.GetAxis("Vertical");
    }
}