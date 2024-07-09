using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviourPun
{
	public LayerMask whatIsTarget; // 공격 대상 레이어

	public float health = 100;
	[HideInInspector] public bool isDead;

	private Transform Target;
	public bool isChase;

	NavMeshAgent nav;

	private void Awake()
	{
		isDead = false;
		nav = GetComponent<NavMeshAgent>();
	}

	private void Start()
	{
		isChase = true;
		StartCoroutine(UpdateTarget());
	}

	IEnumerator UpdateTarget()
	{
		while (!isDead)
		{
			if (Target)
			{
				// 추적 대상 존재 : 경로를 갱신하고 AI 이동을 계속 진행
				nav.isStopped = false;
				nav.SetDestination(Target.transform.position);
			}
			else
			{
				// 추적 대상 없음 : AI 이동 중지
				nav.isStopped = true;

				// 20 유닛의 반지름을 가진 가상의 구를 그렸을때, 구와 겹치는 모든 콜라이더를 가져옴
				// 단, targetLayers에 해당하는 레이어를 가진 콜라이더만 가져오도록 필터링
				Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);



				// 모든 콜라이더들을 순회하면서, 살아있는 플레이어를 찾기
				for (int i = 0; i < colliders.Length; i++)
				{
					// 콜라이더로부터 LivingEntity 컴포넌트 가져오기
					GameObject livingEntity = colliders[i].gameObject;

					// LivingEntity 컴포넌트가 존재하며, 해당 LivingEntity가 살아있다면,
					if (livingEntity.layer == 7)
					{
						// 추적 대상을 해당 LivingEntity로 설정
						Target = livingEntity.transform;

						// for문 루프 즉시 정지
						break;
					}
				}
			}

			// 0.25초 주기로 처리 반복
			yield return new WaitForSeconds(0.25f);
		}
	}

	private void Update()
	{
		if (isChase && Target != null)
		{
			nav.SetDestination(Target.position);
		}
	}

	[PunRPC]
	public void TakeDamage(float damage)
	{
		if (health > 0)
		{
			health -= damage;
			if (health <= 0)
				EnemyDeath();
			else
				Debug.Log("Hit!");
		}
	}

	[PunRPC]
	void EnemyDeath()
	{
		nav.isStopped = true;
		isChase = false;
		nav.enabled = false;

		Debug.Log("Death");

		if (photonView.IsMine)
		{
			PhotonNetwork.Destroy(gameObject);
		}
	}
}
