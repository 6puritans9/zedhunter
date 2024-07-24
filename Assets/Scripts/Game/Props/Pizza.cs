using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pizza : MonoBehaviour
{
	public static Pizza instance;
	public int MyIdx = -1;
	public float rotationSpeed = 30f;

	public float[] timers = { 15f, 30f, 60f };
	private float currentTime;
	public int HP;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		int randomIndex = UnityEngine.Random.Range(0, timers.Length);
		currentTime = timers[randomIndex];
	}

	void Update()
	{
		RotatePizza();
		CheckTimer();
	}

	private void RotatePizza()
	{
		transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
	}

	private void CheckTimer()
	{
		currentTime -= Time.deltaTime;

		if (currentTime <= 0)
		{
			Destroy(gameObject);
		}
	}

	public void TakeDamage(int damage)
	{
		if (HP <= 0) return;

		HP -= damage;

		if (HP <= 0)
		{
			StartCoroutine(DestroyWallWithEffect()); // ���� �ı��� �� ȿ�� ����
		}
	}

	private IEnumerator DestroyWallWithEffect()
	{
		yield return null;
		if(MyIdx != -1)
			PizzaSpawner.Instance.hasPizza[MyIdx] = false;
		// Destroy(gameObject);
		OnPizzaDestroyed();
	}

	private void OnPizzaDestroyed()
	{
		PizzaManager.Instance?.PizzaDestroyed(this);
		Destroy(gameObject);
	}
}