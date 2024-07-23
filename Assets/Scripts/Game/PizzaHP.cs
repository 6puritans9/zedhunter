using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaHP : MonoBehaviour
{
	[SerializeField] int HP;

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
		Destroy(gameObject);
	}
}
