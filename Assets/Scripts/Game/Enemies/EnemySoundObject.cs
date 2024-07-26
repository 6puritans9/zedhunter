using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundObject : MonoBehaviour
{
	public static EnemySoundObject Instance;

	public int zombieType = 1;

	// Sound fields
	public AudioClip pizzaZombieHitSound;
	public AudioClip playerZombieHitSound;

	private AudioSource pizzaZombieAudioSource;
	private AudioSource playerZombieAudioSource;

	private void Awake()
	{
		Instance = this;

		pizzaZombieAudioSource = gameObject.AddComponent<AudioSource>();
		pizzaZombieAudioSource.clip = pizzaZombieHitSound;
		pizzaZombieAudioSource.playOnAwake = false;
		pizzaZombieAudioSource.volume = 0.5f;

		playerZombieAudioSource = gameObject.AddComponent<AudioSource>();
		playerZombieAudioSource.playOnAwake = false;
		playerZombieAudioSource.clip = playerZombieHitSound;
		playerZombieAudioSource.volume = 0.5f;
	}

	private void OnEnable()
	{
		int randomSound = Random.Range(0, 2);

		if (randomSound == 0)
		{
			pizzaZombieAudioSource.Play();
		}
		else
		{
			playerZombieAudioSource.Play();
		}
	}
}
