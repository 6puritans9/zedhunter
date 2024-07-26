using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
	public GameObject[] rockPrefabs;

	public float explosionForce = 500f; // 힘의 크기
	public int numberOfFragments = 20; // 생성할 파편의 개수
	public float fragmentLifetime = 2f; // 파편의 생명주기 (초 단위)

	public Transform TargetPos;

	public void AddForce()
	{
		for (int i = 0; i < numberOfFragments; i++)
		{
			// 파편을 현재 오브젝트의 위치에서 인스턴스화
			GameObject fragment = Instantiate(rockPrefabs[Random.Range(0,3)], TargetPos.position, Quaternion.identity);

			// 파편에 리지드바디 컴포넌트가 있는지 확인
			Rigidbody rb = fragment.GetComponent<Rigidbody>();
			if (rb != null)
			{
				// 랜덤한 방향으로 힘을 가함 (아래로는 튀지 않도록)
				Vector3 explosionDir = (Vector3.up + new Vector3(Random.Range(-1f, 1f), Mathf.Abs(Random.Range(0.5f, 1f)), Random.Range(-1f, 1f))).normalized;
				rb.AddForce(explosionDir * explosionForce);
			}

			// 파편을 2초 뒤에 제거
			Destroy(fragment, fragmentLifetime);
		}
	}
}
