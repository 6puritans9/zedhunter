using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviourPun
{
    public static EnemyHealth Instance;

    public LayerMask whatIsTarget; // ���� ��� ���̾�

    public Coroutine UpdateTargetCorutine;

    public Animator anim;

    [HideInInspector]public Transform Target;
    public bool isChase;
    [HideInInspector]public NavMeshAgent nav;

    public Collider[] colliders;
    float closestDistance = Mathf.Infinity;


    public delegate void EnemyKilledHandler(EnemyHealth enemy);
    public event EnemyKilledHandler OnEnemyKilled;

    public BoxCollider attackRange;

    public float health = 20;
    [HideInInspector] public bool isDead;

    EnemyAttack enemyAttack;

    private void Awake()
    {
        Instance = this;
        enemyAttack = GetComponentInChildren<EnemyAttack>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        isChase = true;
        isDead = false;
        StartCoroutine(UpdateTarget());
    }

    IEnumerator UpdateTarget()
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

                    /*
                    * �÷��̾ Ÿ������ ����
                    * Photon�� RPC ȣ���� ���� ��� Ŭ���̾�Ʈ����
                    * SetTarget�Լ��� ȣ��
                    * ViewID�� Ÿ�� ����ȭ
                    */
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


            if (colliders.Length <= 0)
            {
                isChase = false;
                Target = null;
            }
            else
                isChase = true;

            if (Target)
            {
                // ���� ��� ���� : ��θ� �����ϰ� AI �̵��� ��� ����
                float speed = 0;
                if (!enemyAttack.canAttack)
                {
                    AnimatorStateInfo animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
                    if (animatorStateInfo.IsName("2-attack_inversed_horizontal_right_hand") && animatorStateInfo.normalizedTime >= 0.7f)
                        anim.SetBool("isAttack", false);
                    nav.isStopped = false;
                    speed = nav.velocity.magnitude;
                }
                else
                {
                    nav.isStopped = true;
                    anim.SetBool("isAttack", true);
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

                nav.SetDestination(Target.transform.position);
            }
            else
            {
                nav.isStopped = true;// ���� ��� ���� : AI �̵� ����
                SetRandomDestination();
            }
            // 0.25�� �ֱ�� ó�� �ݺ�
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void Attack()
    {
        if(enemyAttack.canAttack && Target)
        {
            enemyAttack.DoAttack(Target.gameObject);
        }
    }

    public void StopUpdateTargetCorutine()
    {
        if(UpdateTargetCorutine != null)
        {
            StopCoroutine(UpdateTarget());
            UpdateTargetCorutine = null;
        }
    }

    public void StartUpdateTargetCorutine()
    {
        if(UpdateTargetCorutine == null)
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

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (health > 0)
        {
            health -= damage;
            if (PhotonNetwork.IsMasterClient)
            {
                //������ RPC�� EnemyDeath()ȣ���� EnemyPool�� ����ֱ�
                if (health <= 0)
                {
                    photonView.RPC("EnemyDeath", RpcTarget.All);
                    EnemySpawnPool.Instance.enemyPool.Enqueue(this);
                }
            }
            /*else
                Debug.Log("Hit!");*/
        }
    }

    [PunRPC]
    void EnemyDeath()
    {
        if (isDead) return;

        if (photonView.IsMine)
        {
            photonView.RPC("StopAction", RpcTarget.All);

            // OnEnemyKilled �̺�Ʈ ȣ��
            OnEnemyKilled?.Invoke(this);
        }
    }

    [PunRPC]
    public void StopAction()
    {
        isDead = true;
        isChase = false;

        nav.enabled = false;
        anim.enabled = false;
        
        UpdateTargetCorutine = null;
    }

    [PunRPC]
    public void ReStartAction()
    {
        if(nav.enabled == false)
            nav.enabled = true;
        anim.enabled = true;

        isDead = false;
        isChase = true;
        health = 25;

        StartUpdateTargetCorutine();
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
