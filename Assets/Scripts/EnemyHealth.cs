using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviourPun
{
	public LayerMask whatIsTarget; // ���� ��� ���̾�

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
				// ���� ��� ���� : ��θ� �����ϰ� AI �̵��� ��� ����
				nav.isStopped = false;
				nav.SetDestination(Target.transform.position);
			}
			else
			{
				// ���� ��� ���� : AI �̵� ����
				nav.isStopped = true;

				// 20 ������ �������� ���� ������ ���� �׷�����, ���� ��ġ�� ��� �ݶ��̴��� ������
				// ��, targetLayers�� �ش��ϴ� ���̾ ���� �ݶ��̴��� ���������� ���͸�
				Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);



				// ��� �ݶ��̴����� ��ȸ�ϸ鼭, ����ִ� �÷��̾ ã��
				for (int i = 0; i < colliders.Length; i++)
				{
					// �ݶ��̴��κ��� LivingEntity ������Ʈ ��������
					GameObject livingEntity = colliders[i].gameObject;

					// LivingEntity ������Ʈ�� �����ϸ�, �ش� LivingEntity�� ����ִٸ�,
					if (livingEntity.layer == 7)
					{
						// ���� ����� �ش� LivingEntity�� ����
						Target = livingEntity.transform;

						// for�� ���� ��� ����
						break;
					}
				}
			}

			// 0.25�� �ֱ�� ó�� �ݺ�
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
