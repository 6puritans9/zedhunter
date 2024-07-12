using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviourPun
{
    public LayerMask whatIsTarget; // 공격 대상 레이어

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
            // 20 유닛의 반지름을 가진 가상의 구를 그렸을때, 구와 겹치는 모든 콜라이더를 가져옴
            // 단, targetLayers에 해당하는 레이어를 가진 콜라이더만 가져오도록 필터링
            colliders = Physics.OverlapSphere(transform.position, 20, whatIsTarget);
            GameObject closestTarget = null;

            // 모든 콜라이더들을 순회하면서, 살아있는 플레이어를 찾기
            for (int i = 0; i < colliders.Length; i++)
            {
                // 콜라이더로부터 LivingEntity 컴포넌트 가져오기
                GameObject livingEntity = colliders[i].gameObject;

                // LivingEntity 컴포넌트가 존재
                if (livingEntity.layer == 7)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, livingEntity.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = livingEntity;
                    }

                    /*
                    * 플레이어를 타겟으로 설정
                    * Photon의 RPC 호출을 통해 모든 클라이언트에서
                    * SetTarget함수를 호출
                    * ViewID로 타깃 동기화
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
                // 추적 대상 존재 : 경로를 갱신하고 AI 이동을 계속 진행
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
                nav.isStopped = true;// 추적 대상 없음 : AI 이동 중지

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

        // OnEnemyKilled 이벤트 호출
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
