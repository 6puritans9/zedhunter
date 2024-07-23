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
			StartCoroutine(DestroyWallWithEffect()); // 벽이 파괴될 때 효과 적용
		}
	}

	private IEnumerator DestroyWallWithEffect()
	{
		yield return null;
		Destroy(gameObject);
	}
}
