using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
	public GameObject[] rockPrefabs;

	public float explosionForce = 500f; // ���� ũ��
	public int numberOfFragments = 20; // ������ ������ ����
	public float fragmentLifetime = 2f; // ������ �����ֱ� (�� ����)

	public Transform TargetPos;

	public void AddForce()
	{
		for (int i = 0; i < numberOfFragments; i++)
		{
			// ������ ���� ������Ʈ�� ��ġ���� �ν��Ͻ�ȭ
			GameObject fragment = Instantiate(rockPrefabs[Random.Range(0,3)], TargetPos.position, Quaternion.identity);

			// ���� ������ٵ� ������Ʈ�� �ִ��� Ȯ��
			Rigidbody rb = fragment.GetComponent<Rigidbody>();
			if (rb != null)
			{
				// ������ �������� ���� ���� (�Ʒ��δ� Ƣ�� �ʵ���)
				Vector3 explosionDir = (Vector3.up + new Vector3(Random.Range(-1f, 1f), Mathf.Abs(Random.Range(0.5f, 1f)), Random.Range(-1f, 1f))).normalized;
				rb.AddForce(explosionDir * explosionForce);
			}

			// ������ 2�� �ڿ� ����
			Destroy(fragment, fragmentLifetime);
		}
	}
}
