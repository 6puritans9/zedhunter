using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100;
    RagdollMnager ragdollMnager;
    [HideInInspector] public bool isDead;

	public Transform Target;
	public bool isChase;

	NavMeshAgent nav;
	Animator anim;

	private void Awake()
	{
		nav = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();

		Invoke("ChaseStart", 2);
	}


	private void Start()
	{
		ragdollMnager = GetComponent<RagdollMnager>();
	}

	private void Update()
	{
		if (isChase)
		{
			nav.SetDestination(Target.position);
		}
	}

	void ChaseStart()
	{
		isChase = true;
		anim.SetBool("isWalk", true);
	}


	public void TakeDamage(float damage)
    {
        if(health > 0)
        {
            health -= damage;
            if(health <= 0)
			    EnemyDeath();
            else
                Debug.Log("Hit!");
        }
	}

    void EnemyDeath()
    {
        ragdollMnager.TriggerRagdoll();
		isChase = false;
		nav.enabled = false;
		anim.enabled = false;


		Debug.Log("Death");
		StartCoroutine("Destory");
    }

	IEnumerator Destory()
	{
		yield return new WaitForSeconds(3);
		Destroy(gameObject);
	}
}
