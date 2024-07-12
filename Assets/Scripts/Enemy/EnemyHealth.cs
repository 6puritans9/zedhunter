using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviourPun
{
    public LayerMask whatIsTarget; // ���� ��� ���̾�

    Animator anim;

    private Transform Target;
    public bool isChase;
    NavMeshAgent nav;

    public Collider[] colliders;
    float closestDistance = Mathf.Infinity;


    public delegate void EnemyKilledHandler(EnemyHealth enemy);
    public event EnemyKilledHandler OnEnemyKilled;

    public BoxCollider attackRange;

    public float health = 20;
    [HideInInspector] public bool isDead;

    private void Awake()
    {
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
            // 20 ������ �������� ���� ������ ���� �׷�����, ���� ��ġ�� ��� �ݶ��̴��� ������
            // ��, targetLayers�� �ش��ϴ� ���̾ ���� �ݶ��̴��� ���������� ���͸�
            colliders = Physics.OverlapSphere(transform.position, 20, whatIsTarget);
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
                nav.isStopped = true;// ���� ��� ���� : AI �̵� ����

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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
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
