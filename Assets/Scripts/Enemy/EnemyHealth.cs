using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviourPun
{
    public static EnemyHealth Instance;

    public enum ZombieType { Male, Female}
    public ZombieType zombieType;

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
    bool isAttack;

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
            colliders = Physics.OverlapSphere(transform.position, 15, whatIsTarget);
            GameObject closestTarget = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                GameObject targetObject = colliders[i].gameObject;

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

                if (closestTarget != null)
                {
                    Target = closestTarget.transform;
                    photonView.RPC("SetTarget", RpcTarget.AllBuffered, closestTarget.GetComponent<PhotonView>().ViewID);
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
                if(zombieType == ZombieType.Female)
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
                else if(zombieType == ZombieType.Male)
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
                            else if(hitObject.TryGetComponent(out WallHP wall))
                            {
                                enemyAttack.target = wall.gameObject;
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

            //������ RPC�� EnemyDeath()ȣ���� EnemyPool�� ����ֱ�
            if (health <= 0)
            {
                if (PhotonNetwork.IsMasterClient || photonView.IsMine)
                {
                    photonView.RPC("EnemyDeath", RpcTarget.All);

                    // OnEnemyKilled �̺�Ʈ ȣ��
                    OnEnemyKilled?.Invoke(this);
                }
            }
            /*else
                Debug.Log("Hit!");*/
        }
    }

    [PunRPC]
    void EnemyDeath()
    {
        isDead = true;
        if (photonView.IsMine)
            photonView.RPC("StopAction", RpcTarget.All);
    }

    [PunRPC]
    public void StopAction()
    {
        isChase = false;
        nav.enabled = false;
        anim.enabled = false;
        
        UpdateTargetCorutine = null;
        if(isDead)
        {
            if (zombieType == EnemyHealth.ZombieType.Male)
            {
                EnemySpawnPool.Instance.maleZombiePool.Enqueue(this);
                EnemySpawnPool.Instance.maleEnemiesToSpawn -= 1;
            }
            else if (zombieType == EnemyHealth.ZombieType.Female)
            {
                Debug.Log(EnemySpawnPool.Instance.femaleEnemiesToSpawn);
                EnemySpawnPool.Instance.femaleZombiePool.Enqueue(this);
                EnemySpawnPool.Instance.femaleEnemiesToSpawn -= 1;
            }
        }
        gameObject.SetActive(false);
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
