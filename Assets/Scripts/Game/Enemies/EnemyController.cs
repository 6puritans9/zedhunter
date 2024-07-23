using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	public static EnemyController Instance;

	public float jumpHeight = 2f;
	public float jumpSpeed = 2f;
	public float chaseDistance = 5f;
	public float attackDistance = 2.5f;
	public float minJumpDistance = 4f;
	public float maxJumpDistance = 8f;
	public float rotationSpeed = 5f;
	public float jumpCooldown = 3f; // ���� ��Ÿ�� (�� ����)

	public NavMeshAgent agent;
	public Animator animator;
	public Vector3 jumpStartPosition;
	public Vector3 jumpTargetPosition;
	public float jumpStartTime;
	public bool isJumping = false;
	public bool isJumpAnimating = false;

	EnemyHealth enemyHealth;

	private float lastJumpTime; // ������ ���� �ð�

	private void Awake()
	{
		Instance = this;

	}

	void Start()
	{
		enemyHealth = GetComponentInParent<EnemyHealth>();
		agent = GetComponentInParent<NavMeshAgent>();
		animator = GetComponentInParent<Animator>();

		StartCoroutine(Target());
	}

	public void ReStartTarget()
	{
		StartCoroutine(Target());
	}

	IEnumerator Target()
	{
		while (gameObject.activeSelf)
		{
			if (enemyHealth.Target)
			{
				float distanceToPlayer = Vector3.Distance(transform.position, enemyHealth.Target.position);

				if (distanceToPlayer >= minJumpDistance && distanceToPlayer <= maxJumpDistance && !isJumping && Time.time >= lastJumpTime + jumpCooldown)
				{
					// �÷��̾ ���� �Ÿ� ���� ������ ����
					StartJump();
				}
			}

			if (isJumping)
			{
				PerformJump();
			}

			yield return new WaitForSeconds(0.25f);
		}
	}


	void StartJump()
	{
		isJumping = true;
		isJumpAnimating = true;
		agent.isStopped = true;
		jumpStartPosition = transform.position;
		jumpTargetPosition = enemyHealth.Target.position;
		jumpStartTime = Time.time;
		lastJumpTime = Time.time; // ������ ���� �ð��� ���� �ð����� ����

		// ���� ���̸� ����� ������ ���� ���
		Vector3 jumpDirection = (jumpTargetPosition - jumpStartPosition).normalized;
		float jumpDistance = Mathf.Min(Vector3.Distance(jumpStartPosition, jumpTargetPosition), maxJumpDistance);

		//��ǥ ��ġ�� ���� �ָ� ����
		float extraDistance = 2.0f; //�߰��� ������ �Ÿ�(���ϴ� ��ŭ ����)
		jumpTargetPosition = jumpStartPosition + jumpDirection * (jumpDistance + extraDistance);
		jumpTargetPosition.y = jumpStartPosition.y;
	}

	void PerformJump()
	{
		float elapsedTime = Time.time - jumpStartTime;
		float jumpProgress = elapsedTime / jumpSpeed;
		animator.SetBool("isJumping", isJumpAnimating);
		if (jumpProgress < 1f)
		{
			// ������ � ���
			Vector3 currentPosition = Vector3.Lerp(jumpStartPosition, jumpTargetPosition, jumpProgress);
			currentPosition.y = jumpStartPosition.y + Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight;

			transform.position = currentPosition;
		}
		else
		{
			LandJump();
		}
	}

	void LandJump()
	{
		isJumping = false;
		agent.isStopped = false;

		isJumpAnimating = false;
		animator.SetFloat("speed", 0);
		animator.SetBool("isJumping", isJumpAnimating);
	}
}
