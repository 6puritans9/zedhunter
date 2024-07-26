using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSoundEffect : MonoBehaviour
{
	WallHP wallHP;
	BoxDamage boxDamage;

	public AudioClip collisionSound; // 충돌 소리 클립
	public AudioClip fallingSound; // 낙하 소리 클립
	public AudioClip overEnemySound; // 적을 깔아버릴 때 소리 클립
	public AudioClip explosionSound; //폭발 소리 클립
	public AudioClip retroSound;    //폭발 전 경고음

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
		// AudioSource 컴포넌트를 추가하고 설정합니다.
		impactToGroundAudioSource = gameObject.AddComponent<AudioSource>();
		impactToGroundAudioSource.clip = collisionSound;
		impactToGroundAudioSource.loop = false;
		impactToGroundAudioSource.playOnAwake = false;
		impactToGroundAudioSource.volume = 2.0f; // 기본 볼륨 설정

		// AudioSource 컴포넌트를 추가하고 설정합니다.
		impactToZombieAudioSource = gameObject.AddComponent<AudioSource>();
		impactToZombieAudioSource.clip = overEnemySound;
		impactToZombieAudioSource.loop = false;
		impactToZombieAudioSource.playOnAwake = false;
		impactToZombieAudioSource.volume = 2.0f; // 기본 볼륨 설정

		// 낙하 소리용 AudioSource 컴포넌트를 추가하고 설정합니다.
		fallingAudioSource = gameObject.AddComponent<AudioSource>();
		fallingAudioSource.clip = fallingSound;
		fallingAudioSource.loop = true;
		fallingAudioSource.playOnAwake = false;
		fallingAudioSource.volume = 2.0f; // 기본 볼륨 설정

		// 폭발 소리용 AudioSource 컴포넌트를 추가하고 설정합니다.
		explosionAudioSource = gameObject.AddComponent<AudioSource>();
		explosionAudioSource.clip = explosionSound;
		explosionAudioSource.loop = false;
		explosionAudioSource.playOnAwake = false;
		explosionAudioSource.volume = 1.0f;

		// 폭발전 경고음
		retroAudioSource = gameObject.AddComponent<AudioSource>();
		retroAudioSource.clip = retroSound;
		retroAudioSource.loop = false;
		retroAudioSource.playOnAwake = false;
		retroAudioSource.volume = 1.0f;

		// 낙하 소리 재생 시작
		fallingAudioSource.Play();
	}

	void OnCollisionEnter(Collision collision)
	{
		//박스가 "Enemy" 레이어와 충돌할 때 소리 재생
		if (!boxDamage.groundCheck && collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && !OnlyOnePlay)
		{
			OnlyOnePlay = true;
			// 낙하 소리 중지
			fallingAudioSource.Stop();
		}
		// 박스가 "Ground" 레이어와 충돌할 때 소리 재생
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			// 낙하 소리 중지
			fallingAudioSource.Stop();
		}
	}

	public void ImpactToGround()
	{
		// 충돌 소리 재생
		impactToGroundAudioSource.PlayOneShot(impactToGroundAudioSource.clip);
	}

	public void ImpactToZombie()
	{
		impactToZombieAudioSource.PlayOneShot(impactToZombieAudioSource.clip);
	}
}
