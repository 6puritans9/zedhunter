using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSoundEffect : MonoBehaviour
{
	WallHP wallHP;

	public AudioClip collisionSound; // 충돌 소리 클립
	public AudioClip fallingSound; // 낙하 소리 클립
	public AudioClip explosionSound; //폭발 소리 클립

	private AudioSource audioSource;
	private AudioSource fallingAudioSource;
	[HideInInspector]public AudioSource explosionAudioSource;

	private void Awake()
	{
		wallHP = GetComponent<WallHP>();
	}

	void Start()
	{
		// AudioSource 컴포넌트를 추가하고 설정합니다.
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = collisionSound;
		audioSource.playOnAwake = false;
		audioSource.volume = 2.0f; // 기본 볼륨 설정

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

		// 낙하 소리 재생 시작
		fallingAudioSource.Play();
	}

	void OnCollisionEnter(Collision collision)
	{
		// 박스가 "Ground" 레이어와 충돌할 때 소리 재생
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			// 낙하 소리 중지
			fallingAudioSource.Stop();

			// 충돌 소리 재생
			audioSource.Play();
		}
	}

}
