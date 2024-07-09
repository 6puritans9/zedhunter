using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviourPun
{
	public delegate void EnemyKilledHandler(EnemyHealth enemy);
	public event EnemyKilledHandler OnEnemyKilled;

	public LayerMask whatIsTarget; // ���� ��� ���̾�

	public float health = 100;
	[HideInInspector] public bool isDead;
	public float attackDistance = 3.3f;  // ���� �Ÿ� ����
	private bool isAttacking = false;
	private bool canAttack = true;

	private Transform Target;
	public bool isChase;
	NavMeshAgent nav;

	Animator anim;

	private void Awake()
	{
		isDead = false;
		nav = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
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
				float speed = nav.velocity.magnitude;

				if (speed > 0.1f)
				{
					anim.SetBool("isWalking", true);
					anim.SetFloat("speed", speed);
				}
				else
				{
					anim.SetBool("isWalking", false);
					anim.SetFloat("speed", 0f);
				}

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

					// LivingEntity ������Ʈ�� ����
					if (livingEntity.layer == 7)
					{
						/*
                         * �÷��̾ Ÿ������ ����
                         * Photon�� RPC ȣ���� ���� ��� Ŭ���̾�Ʈ����
                         * SetTarget�Լ��� ȣ��
                         * ViewID�� Ÿ�� ����ȭ
                         */
						photonView.RPC(
							"SetTarget",
							RpcTarget.AllBuffered,
							livingEntity.GetComponent<PhotonView>().ViewID);
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
		if (isDead) return;

		isDead = true;
		nav.isStopped = true;
		isChase = false;
		nav.enabled = false;

		Debug.Log("Death");

		// OnEnemyKilled �̺�Ʈ ȣ��
		OnEnemyKilled?.Invoke(this);

		if (photonView.IsMine)
		{
			PhotonNetwork.Destroy(gameObject);
		}
	}

	[PunRPC]
	void SetTarget(int targetViewID)
	{
		PhotonView targetPhotonView = PhotonView.Find(targetViewID);
		if (targetPhotonView != null)
		{
			Target = targetPhotonView.transform;
		}
	}
}
