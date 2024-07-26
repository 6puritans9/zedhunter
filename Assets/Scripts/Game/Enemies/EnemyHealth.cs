using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static CartoonFX.CFXR_Effect;

public class EnemyHealth : MonoBehaviour
	{
		private GameManager _gameManager; 
	
	public static EnemyHealth Instance;

	public enum ZombieType { PlayerZombie, PizzaZombie, Boss }
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

	public float MaxHealth;
	[HideInInspector] public float health;
	[HideInInspector] public bool isDead;

	EnemyAttack enemyAttack;
	bool isAttack;

	int OriginNavSpeed;

	// === BossParam === 
	/*public float chaseDistance = 5f;
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
	private bool isJumpAnimating = false;*/
	public AudioClip bossDeathSoundClip;
	public AudioClip bossAttackSoundClip;
	private AudioSource bossDeathAudioSource;
	private AudioSource bossAttackAudioSource;
	// === BossParam === 

	public GameObject zombie4Prefab;

	private void Awake()
	{
		Instance = this;
		_gameManager = FindObjectOfType<GameManager>();

		isDead = false;

		bossDeathAudioSource = gameObject.AddComponent<AudioSource>();
		bossDeathAudioSource.loop = false;
		bossDeathAudioSource.playOnAwake = false;
		bossDeathAudioSource.clip = bossDeathSoundClip;

		bossAttackAudioSource = gameObject.AddComponent<AudioSource>();
		bossAttackAudioSource.loop = false;
		bossAttackAudioSource.playOnAwake = false;
		bossAttackAudioSource.clip = bossAttackSoundClip;
		bossAttackAudioSource.volume = 1;


		enemyAttack = GetComponentInChildren<EnemyAttack>();
		nav = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
	}

	private void Start()
	{
		isAttack = false;
		isChase = true;

		if (zombieType == ZombieType.PizzaZombie)
			nav.speed = OriginNavSpeed = 4;
		else if (zombieType == ZombieType.PlayerZombie)
			nav.speed = OriginNavSpeed = 5;
		else
			nav.speed = OriginNavSpeed = 7;

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
				case ZombieType.PizzaZombie:
					colliders = Physics.OverlapSphere(transform.position, 100, whatIsTarget);
					break;
				case ZombieType.PlayerZombie:
					colliders = Physics.OverlapSphere(transform.position, 100, whatIsTarget);
					break;
				case ZombieType.Boss:
					colliders = Physics.OverlapSphere(transform.position, 100, whatIsTarget);
					break;
			}

			for (int i = 0; i < colliders.Length; i++)
			{
				GameObject targetObject = colliders[i].gameObject;

				//���� Ÿ�Կ� ���� �ٸ��� Ÿ����
				if (zombieType == ZombieType.PizzaZombie && targetObject.layer == LayerMask.NameToLayer("Pizza"))
				{
					float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
					if (distanceToTarget < closestDistance)
					{
						closestDistance = distanceToTarget;
						closestTarget = targetObject;
					}
				}
				else if (zombieType == ZombieType.PlayerZombie && (targetObject.layer == LayerMask.NameToLayer("Player") || targetObject.layer == LayerMask.NameToLayer("Wall")))
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
				float targetRadius = 1f;
				float targetRange = 1f;
				if (zombieType == ZombieType.PizzaZombie)
				{
					RaycastHit[] rayHits = Physics.SphereCastAll(
						transform.position,
						targetRadius,
						transform.forward,
						targetRange,
						LayerMask.GetMask("Pizza"));

					if (rayHits.Length > 0 && !isAttack)
					{
						foreach (RaycastHit hit in rayHits)
						{
							GameObject hitObject = hit.collider.gameObject;
							if (hitObject.TryGetComponent(out Pizza pizza))
							{
								enemyAttack.target = pizza.gameObject;
								break;
							}
						}
						StartCoroutine(Attack());
					}
				}
				else if (zombieType == ZombieType.PlayerZombie)
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
					nav.speed = OriginNavSpeed;
				}
				else
				{
					nav.isStopped = true;
					nav.speed = 0;
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

	public void PlayerCamShake()
	{
		ShakeCameraPos.Instance?.TriggerShake();
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

			if (health <= 0)
			{
				EnemyDeath();

				OnEnemyKilled?.Invoke(this);
			}
			/*else
                Debug.Log("Hit!");*/
		}
	}

	void EnemyDeath()
	{
		isDead = true;
		if (zombieType == ZombieType.Boss)
		{
			_gameManager.AddBossZombieScore();
			anim.SetTrigger("death");
			bossDeathAudioSource.Play();
			Invoke("StopAction", 3.5f);
		}
		else
			StopAction();
	}

	private void SetAnimationsFalse()
		{
			isChase = false;
			nav.enabled = false;
			anim.enabled = false;			
		}

	public void StopAction()
	{
		if (isDead && (zombieType == ZombieType.PlayerZombie || zombieType == ZombieType.PizzaZombie))
		{
			if (zombie4Prefab != null)
			{
				if (zombieType == EnemyHealth.ZombieType.PlayerZombie)
				{
					_gameManager.AddPlayerZombieScore();
					SetAnimationsFalse();
					EnemySpawnPool.Instance.playerZombiePool.Enqueue(this);
					EnemySpawnPool.Instance.pizzaEnemiesToSpawn -= 1;
				}
				else if (zombieType == EnemyHealth.ZombieType.PizzaZombie)
				{
					_gameManager.AddPizzaZombieScore();
					SetAnimationsFalse();
					EnemySpawnPool.Instance.pizzaZombiePool.Enqueue(this);
					EnemySpawnPool.Instance.playerEnemiesToSpawn -= 1;
				}

				GameObject zombie4Instance = Instantiate(zombie4Prefab, transform.position, transform.rotation);
				zombie4Instance.SetActive(true);

				Renderer renderer = zombie4Instance.GetComponent<Renderer>();
				if (renderer != null)
				{
					renderer.enabled = true;
				}

				Destroy(zombie4Instance, 2f);
			}
		}
		else if (isDead && zombieType == EnemyHealth.ZombieType.Boss)
		{
			// _gameManager.AddBossZombieScore();
			SetAnimationsFalse();
			EnemySpawnPool.Instance.bossZombiePool.Enqueue(this);
			EnemySpawnPool.Instance.bossEnemiesToSpawn -= 1;
		}
		gameObject.SetActive(false);
	}

	public void ReStartAction()
	{
		if (nav.enabled == false)
			nav.enabled = true;
		//nav.speed = OriginNavSpeed;

		anim.enabled = true;

		if (isDead)
			if (zombieType == ZombieType.Boss)
				EnemyController.Instance.ReStartTarget();
		isDead = false;
		isChase = true;
		isAttack = false;

		switch (zombieType)
		{
			case ZombieType.PizzaZombie:
				health = MaxHealth;
				break;
			case ZombieType.PlayerZombie:
				health = MaxHealth;
				break;
			case ZombieType.Boss:
				health = MaxHealth;
				break;
		}

		StartCoroutine(UpdateTarget());
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

	public void BossAttackSound()
	{
		bossAttackAudioSource.Play();
	}
}
