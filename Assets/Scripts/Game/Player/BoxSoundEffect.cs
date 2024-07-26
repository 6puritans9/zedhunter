using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSoundEffect : MonoBehaviour
{
	WallHP wallHP;
	BoxDamage boxDamage;

	public AudioClip collisionSound; // �浹 �Ҹ� Ŭ��
	public AudioClip fallingSound; // ���� �Ҹ� Ŭ��
	public AudioClip overEnemySound; // ���� ��ƹ��� �� �Ҹ� Ŭ��
	public AudioClip explosionSound; //���� �Ҹ� Ŭ��
	public AudioClip retroSound;    //���� �� �����

	private AudioSource impactToGroundAudioSource;
	private AudioSource impactToZombieAudioSource;
	private AudioSource fallingAudioSource;
	[HideInInspector] public AudioSource retroAudioSource;
	[HideInInspector] public AudioSource explosionAudioSource;

	bool OnlyOnePlay;

	private void OnEnable()
	{
		OnlyOnePlay = false;
	}

	private void Awake()
	{
		wallHP = GetComponent<WallHP>();
		boxDamage = GetComponent<BoxDamage>();
	}

	void Start()
	{
		// AudioSource ������Ʈ�� �߰��ϰ� �����մϴ�.
		impactToGroundAudioSource = gameObject.AddComponent<AudioSource>();
		impactToGroundAudioSource.clip = collisionSound;
		impactToGroundAudioSource.loop = false;
		impactToGroundAudioSource.playOnAwake = false;
		impactToGroundAudioSource.volume = 2.0f; // �⺻ ���� ����

		// AudioSource ������Ʈ�� �߰��ϰ� �����մϴ�.
		impactToZombieAudioSource = gameObject.AddComponent<AudioSource>();
		impactToZombieAudioSource.clip = overEnemySound;
		impactToZombieAudioSource.loop = false;
		impactToZombieAudioSource.playOnAwake = false;
		impactToZombieAudioSource.volume = 2.0f; // �⺻ ���� ����

		// ���� �Ҹ��� AudioSource ������Ʈ�� �߰��ϰ� �����մϴ�.
		fallingAudioSource = gameObject.AddComponent<AudioSource>();
		fallingAudioSource.clip = fallingSound;
		fallingAudioSource.loop = true;
		fallingAudioSource.playOnAwake = false;
		fallingAudioSource.volume = 2.0f; // �⺻ ���� ����

		// ���� �Ҹ��� AudioSource ������Ʈ�� �߰��ϰ� �����մϴ�.
		explosionAudioSource = gameObject.AddComponent<AudioSource>();
		explosionAudioSource.clip = explosionSound;
		explosionAudioSource.loop = false;
		explosionAudioSource.playOnAwake = false;
		explosionAudioSource.volume = 1.0f;

		// ������ �����
		retroAudioSource = gameObject.AddComponent<AudioSource>();
		retroAudioSource.clip = retroSound;
		retroAudioSource.loop = false;
		retroAudioSource.playOnAwake = false;
		retroAudioSource.volume = 1.0f;

		// ���� �Ҹ� ��� ����
		fallingAudioSource.Play();
	}

	void OnCollisionEnter(Collision collision)
	{
		//�ڽ��� "Enemy" ���̾�� �浹�� �� �Ҹ� ���
		if (!boxDamage.groundCheck && collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && !OnlyOnePlay)
		{
			OnlyOnePlay = true;
			// ���� �Ҹ� ����
			fallingAudioSource.Stop();
		}
		// �ڽ��� "Ground" ���̾�� �浹�� �� �Ҹ� ���
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			// ���� �Ҹ� ����
			fallingAudioSource.Stop();
		}
	}

	public void ImpactToGround()
	{
		// �浹 �Ҹ� ���
		impactToGroundAudioSource.PlayOneShot(impactToGroundAudioSource.clip);
	}

	public void ImpactToZombie()
	{
		impactToZombieAudioSource.PlayOneShot(impactToZombieAudioSource.clip);
	}
}
