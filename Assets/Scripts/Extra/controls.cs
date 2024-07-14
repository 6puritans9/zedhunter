using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class controls : MonoBehaviour
{
	[SerializeField]
	private Camera cam;
	[SerializeField]
	private float moveSpeed = 5f; // 이동 속도
	[SerializeField]
	private GameObject gun;
	[SerializeField]
	private float rotationSpeed = 5f; // 회전 속도를 조절하기 위한 변수 추가

	private Rigidbody rb;
	private CapsuleCollider capsuleCollider;
	private Vector2 moveInput;
	
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		capsuleCollider = GetComponent<CapsuleCollider>();
	}

	void FixedUpdate()
	{
		Move();
	}

	private void Move()
	{
		Vector3 moveDirection = cam.transform.right * moveInput.x + cam.transform.forward * moveInput.y;
		moveDirection.y = 0; // y축 방향 제거
		Vector3 moveVelocity = moveDirection.normalized * moveSpeed;

		// Rigidbody를 사용한 이동 처리
		rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);

		if (moveDirection.sqrMagnitude > 0.1f)
		{
			Rotate(moveDirection);
		}

		gun.transform.localRotation = Quaternion.Euler(new Vector3(cam.transform.rotation.eulerAngles.x, 0, 0));
	}

	private void Rotate(Vector3 targetDirection)
	{
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
		rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
	}

	public void GetMoveInput(InputAction.CallbackContext context)
	{
		moveInput = context.ReadValue<Vector2>();
	}

	public void OnFire(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Vector3 forward = cam.transform.forward;
			forward.y = 0;
			forward.Normalize();
			Rotate(forward);

			Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay(center);

			Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue, 2);

			if (Physics.Raycast(ray, out hit))
			{
				Debug.Log(hit.transform.name);
			}
		}
	}
}
