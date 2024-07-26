using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSoundEffect : MonoBehaviour
{
	WallHP wallHP;

	public AudioClip collisionSound; // �浹 �Ҹ� Ŭ��
	public AudioClip fallingSound; // ���� �Ҹ� Ŭ��
	public AudioClip overEnemySound; // ���� ��ƹ��� �� �Ҹ� Ŭ��
	public AudioClip explosionSound; //���� �Ҹ� Ŭ��

	private AudioSource audioSource;
	private AudioSource fallingAudioSource;
	[HideInInspector]public AudioSource explosionAudioSource;

	bool OnlyOnePlay;

	private void OnEnable()
	{
		OnlyOnePlay = false;
	}

	private void Awake()
	{
		wallHP = GetComponent<WallHP>();
	}

	void Start()
	{
		// AudioSource ������Ʈ�� �߰��ϰ� �����մϴ�.
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = collisionSound;
		audioSource.playOnAwake = false;
		audioSource.volume = 2.0f; // �⺻ ���� ����

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

		// ���� �Ҹ� ��� ����
		fallingAudioSource.Play();
	}

	void OnCollisionEnter(Collision collision)
	{
		//�ڽ��� "Enemy" ���̾�� �浹�� �� �Ҹ� ���
		if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && !OnlyOnePlay)
		{
			OnlyOnePlay = true;
			// ���� �Ҹ� ����
			fallingAudioSource.Stop();
			audioSource.clip = overEnemySound;
			audioSource.PlayOneShot(audioSource.clip);
		}
		// �ڽ��� "Ground" ���̾�� �浹�� �� �Ҹ� ���
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			// ���� �Ҹ� ����
			fallingAudioSource.Stop();

			audioSource.clip = collisionSound;
			// �浹 �Ҹ� ���
			audioSource.Play();
		}
	}

}
