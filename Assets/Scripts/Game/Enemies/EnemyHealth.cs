using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
	public static EnemyHealth Instance;

	public enum ZombieType { Male, Female, Boss }
	public ZombieType zombieType;

	public LayerMask whatIsTarget; // ���� ��� ���̾�

	public Coroutine UpdateTargetCorutine;

	public Animator anim;

	[HideInInspector] public Transform Target;
	public bool isChase;
	[HideInInspector] public NavMeshAgent nav;


	public Collider[] colliders;
	float closestDistance = Mathf.Infinity;


	public delegate void EnemyKilledHandler(EnemyHealth enemy);
	public event EnemyKilledHandler OnEnemyKilled;

	public BoxCollider attackRange;

	public float health = 20;
	[HideInInspector] public bool isDead;

	EnemyAttack enemyAttack;
	bool isAttack;

	// === BossParam === 
	public float chaseDistance = 5f;
	public float minJumpDistance = 4f;
	public float maxJumpDistance = 8f;
	public float jumpSpeed = 2f;
	public float jumpHeight = 2f;
	public float jumpCooldown = 3f; // ���� ��Ÿ�� (�� ����)

	private Vector3 jumpStartPosition;
	private Vector3 jumpTargetPosition;
	private float jumpStartTime;
	private float lastJumpTime; // ������ ���� �ð�
	private bool isJumping = false;
	private bool isJumpAnimating = false;
	// === BossParam === 

	private void Awake()
	{
		Instance = this;

		isDead = false;

		enemyAttack = GetComponentInChildren<EnemyAttack>();
		nav = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
	}

	private void Start()
	{
		isAttack = false;
		isChase = true;


		StartCoroutine(UpdateTarget());
	}



	IEnumerator UpdateTarget()
	{
		while (!isDead)
		{
			closestDistance = Mathf.Infinity;
			GameObject closestTarget = null;
			switch (zombieType)
			{
				case ZombieType.Female:
					colliders = Physics.OverlapSphere(transform.position, 10, whatIsTarget);
					break;
				case ZombieType.Male:
					colliders = Physics.OverlapSphere(transform.position, 15, whatIsTarget);
					break;
				case ZombieType.Boss:
					colliders = Physics.OverlapSphere(transform.position, 25, whatIsTarget);
					break;
			}

			for (int i = 0; i < colliders.Length; i++)
			{
				GameObject targetObject = colliders[i].gameObject;

				//���� Ÿ�Կ� ���� �ٸ��� Ÿ����
				if (zombieType == ZombieType.Female && targetObject.layer == LayerMask.NameToLayer("Player"))
				{
					float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
					if (distanceToTarget < closestDistance)
					{
						closestDistance = distanceToTarget;
						closestTarget = targetObject;
					}
				}
				else if (zombieType == ZombieType.Male && (targetObject.layer == LayerMask.NameToLayer("Player") || targetObject.layer == LayerMask.NameToLayer("Wall")))
				{
					float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
					if (distanceToTarget < closestDistance)
					{
						closestDistance = distanceToTarget;
						closestTarget = targetObject;
					}
				}
				else if (zombieType == ZombieType.Boss && targetObject.layer == LayerMask.NameToLayer("Player"))
				{
					float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);

					if (distanceToTarget < closestDistance)
					{
						closestDistance = distanceToTarget;
						closestTarget = targetObject;
					}
				}

				if (closestTarget != null)
				{
					Target = closestTarget.transform;
					SetTarget(closestTarget);
				}
			}

			if (colliders.Length <= 0)
			{
				isChase = false;
				Target = null;
			}
			else
			{
				isChase = true;
			}

			if (Target)
			{
				float speed = 0;
				float targetRadius = 0.5f;
				float targetRange = 0.5f;
				if (zombieType == ZombieType.Female)
				{
					RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

					if (rayHits.Length > 0 && !isAttack)
					{
						foreach (RaycastHit hit in rayHits)
						{
							GameObject hitObject = hit.collider.gameObject;
							if (hitObject.TryGetComponent(out ActionStateManager player))
							{
								enemyAttack.target = player.gameObject;
								break;
							}
						}
						StartCoroutine(Attack());
					}
				}
				else if (zombieType == ZombieType.Male)
				{
					RaycastHit[] rayHits = Physics.SphereCastAll(
						transform.position,
						targetRadius,
						transform.forward,
						targetRange,
						LayerMask.GetMask("Player") | LayerMask.GetMask("Wall"));

					if (rayHits.Length > 0 && !isAttack)
					{
						foreach (RaycastHit hit in rayHits)
						{
							GameObject hitObject = hit.collider.gameObject;
							if (hitObject.TryGetComponent(out ActionStateManager player))
							{
								enemyAttack.target = player.gameObject;
								break;
							}
							else if (hitObject.TryGetComponent(out WallHP wall))
							{
								enemyAttack.target = wall.gameObject;
								break;
							}
						}
						StartCoroutine(Attack());
					}
				}
				else if (zombieType == ZombieType.Boss)
				{
					RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

					if (rayHits.Length > 0 && !isAttack)
					{
						foreach (RaycastHit hit in rayHits)
						{
							GameObject hitObject = hit.collider.gameObject;
							if (hitObject.TryGetComponent(out ActionStateManager player))
							{
								enemyAttack.target = player.gameObject;
								break;
							}
						}
						StartCoroutine(Attack());
					}
				}


				if (!isAttack)
				{
					nav.isStopped = false;
					speed = nav.velocity.magnitude;
				}
				else
				{
					nav.isStopped = true;
				}

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

				if (isChase)
				{
					nav.SetDestination(Target.transform.position);
				}
			}
			else
			{
				nav.isStopped = true;
				anim.SetBool("isWalking", false);
				anim.SetFloat("speed", 0f);
			}

			yield return new WaitForSeconds(0.25f);
		}
	}


	/*IEnumerator UpdateTarget()
    {
        while (!isDead)
        {
            // �� ���� ���� �� closestDistance�� �ʱ�ȭ
            closestDistance = Mathf.Infinity;
            // 20 ������ �������� ���� ������ ���� �׷�����, ���� ��ġ�� ��� �ݶ��̴��� ������
            // ��, targetLayers�� �ش��ϴ� ���̾ ���� �ݶ��̴��� ���������� ���͸�
            colliders = Physics.OverlapSphere(transform.position, 15, whatIsTarget);
            GameObject closestTarget = null;

            // ��� �ݶ��̴����� ��ȸ�ϸ鼭, ����ִ� �÷��̾ ã��
            for (int i = 0; i < colliders.Length; i++)
            {
                // �ݶ��̴��κ��� LivingEntity ������Ʈ ��������
                GameObject livingEntity = colliders[i].gameObject;

                // LivingEntity ������Ʈ�� ����
                if (livingEntity.layer == 7)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, livingEntity.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = livingEntity;
                    }

                    *//*
                    * �÷��̾ Ÿ������ ����
                    * Photon�� RPC ȣ���� ���� ��� Ŭ���̾�Ʈ����
                    * SetTarget�Լ��� ȣ��
                    * ViewID�� Ÿ�� ����ȭ
                    *//*
                    if (closestTarget != null)
                    {
                        Target = closestTarget.transform;
                        photonView.RPC(
                            "SetTarget",
                            RpcTarget.AllBuffered,
                            closestTarget.GetComponent<PhotonView>().ViewID);
                    }
                }
            }

            //���������Ǻ�
            if (colliders.Length <= 0)
            {
                isChase = false;
                Target = null;
            }
            else
                isChase = true;

            if (Target)
            {
                float speed = 0;

                float targetRadius = 0.5f;
                float targetRange = 0.5f;
                RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                                                         targetRadius, transform.forward,
                                                         targetRange, LayerMask.GetMask("Player"));
                
                if (rayHits.Length > 0 && !isAttack)
                {
                    foreach (RaycastHit hit in rayHits)
                    {
                        GameObject hitObject = hit.collider.gameObject;
                        if(hitObject.TryGetComponent(out ActionStateManager player))
                        {
                            enemyAttack.targetPlayer = player.gameObject;
                            break;
                        }
                    }
                    StartCoroutine(Attack());
                }

                //�������̸� ��� nav��� ����
                if (!isAttack)
                {
                    nav.isStopped = false;
                    speed = nav.velocity.magnitude;
                }
                else
                {
                    nav.isStopped = true;
                }

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

                if(isChase)
                    nav.SetDestination(Target.transform.position);
            }
            else
            {
                nav.isStopped = true;// ���� ��� ���� : AI �̵� ����
                //SetRandomDestination();
            }
            // 0.25�� �ֱ�� ó�� �ݺ�
            yield return new WaitForSeconds(0.25f);
        }
    }*/

	IEnumerator Attack()
	{
		isChase = false;
		isAttack = true;
		anim.SetBool("isAttack", true);

		yield return new WaitForSeconds(1f);

		isChase = true;
		isAttack = false;
		anim.SetBool("isAttack", false);
	}

	public void DoAttack()
	{
		if (enemyAttack != null)
			enemyAttack.DoAttack();
	}

	public void StopUpdateTargetCorutine()
	{
		if (UpdateTargetCorutine != null)
		{
			StopCoroutine(UpdateTarget());
			UpdateTargetCorutine = null;
		}
	}

	public void StartUpdateTargetCorutine()
	{
		if (UpdateTargetCorutine == null)
		{
			UpdateTargetCorutine = StartCoroutine(UpdateTarget());
		}
	}



	private void SetRandomDestination()
	{
		anim.SetBool("isWalking", true);
		anim.SetFloat("speed", nav.velocity.magnitude);


		Vector3 randomDir = Random.insideUnitCircle * 2;
		randomDir += transform.position;
		NavMeshHit navHit;
		NavMesh.SamplePosition(randomDir, out navHit, 1, NavMesh.AllAreas);

		nav.SetDestination(navHit.position);
		nav.isStopped = false;
	}

	public void TakeDamage(float damage)
	{
		if (health > 0)
		{
			health -= damage;

			//������ RPC�� EnemyDeath()ȣ���� EnemyPool�� ����ֱ�
			if (health <= 0)
			{
				EnemyDeath();

				// OnEnemyKilled �̺�Ʈ ȣ��
				OnEnemyKilled?.Invoke(this);
			}
			/*else
                Debug.Log("Hit!");*/
		}
	}

	void EnemyDeath()
	{
		isDead = true;
		StopAction();
	}

	public void StopAction()
	{
		isChase = false;
		nav.enabled = false;
		anim.enabled = false;

		UpdateTargetCorutine = null;
		if (isDead)
		{
			if (zombieType == EnemyHealth.ZombieType.Male)
			{
				EnemySpawnPool.Instance.maleZombiePool.Enqueue(this);
				EnemySpawnPool.Instance.maleEnemiesToSpawn -= 1;
			}
			else if (zombieType == EnemyHealth.ZombieType.Female)
			{
				EnemySpawnPool.Instance.femaleZombiePool.Enqueue(this);
				EnemySpawnPool.Instance.femaleEnemiesToSpawn -= 1;
			}
			else if (zombieType == EnemyHealth.ZombieType.Boss)
			{
				EnemySpawnPool.Instance.bossZombiePool.Enqueue(this);
				EnemySpawnPool.Instance.bossEnemiesToSpawn -= 1;
			}
		}
		gameObject.SetActive(false);
	}

	public void ReStartAction()
	{
		if (nav.enabled == false)
			nav.enabled = true;
		anim.enabled = true;
		if (isDead)
			if (zombieType == ZombieType.Boss)
				EnemyController.Instance.ReStartTarget();
		isDead = false;
		isChase = true;

		switch (zombieType)
		{
			case ZombieType.Female:
				health = 25;
				break;
			case ZombieType.Male:
				health = 80;
				break;
			case ZombieType.Boss:
				health = 300;
				break;
		}

		StartUpdateTargetCorutine();
		/*if (zombieType == EnemyHealth.ZombieType.Boss)
            enemyJump.RestartCorutine();*/
	}

	void SetTarget(GameObject target)
	{
		if (target != null)
		{
			Target = target.transform;
		}
	}
}
