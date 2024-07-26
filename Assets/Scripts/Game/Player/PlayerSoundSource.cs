using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundSource : MonoBehaviour
{
	public static PlayerSoundSource Instance;

	public AudioClip takeDamage1;
    public AudioClip takeDamage2;
    public AudioClip takeDamage3;

	public AudioClip takeDamage4;
	public AudioClip takeDamage5;

	public AudioClip useKafkaSkill1;
	public AudioClip useServalSkill2;
	public AudioClip useServalSkill3;

	public AudioClip useYanqingSkill1;
	public AudioClip useYanqingSkill2;

	AudioSource takeDamageAudioSource;
	AudioSource useSkillAudioSource;

	private void Awake()
	{
		Instance = this;	

		takeDamageAudioSource = gameObject.AddComponent<AudioSource>();
		takeDamageAudioSource.volume = 1.0f;
		takeDamageAudioSource.playOnAwake = false;
		takeDamageAudioSource.loop = false;

		useSkillAudioSource = gameObject.AddComponent<AudioSource>();
		useSkillAudioSource.volume = 1.0f;
		useSkillAudioSource.playOnAwake = false;
		useSkillAudioSource.loop = false;
	}

	public void GetTakeDamageServalKafkaSound()
	{
		int i = Random.Range(0, 3);
		switch (i)
		{
			case 0:
				TakeDamageSound1();
				break;
			case 1:
				TakeDamageSound2();
				break;
			case 2:
				TakeDamageSound3();
				break;
		}
	}

	public void GetTakeDamageYanqingSound()
	{
		int i = Random.Range(0, 2);
		switch (i)
		{
			case 0:
				TakeDamageYanqingSound1();
				break;
			case 1:
				TakeDamageYanqingSound2();
				break;
		}
	}

	public void GetUseKafkaSkillSound()
	{
		UseSkill1();
	}

	public void GetUseServalSkillSound()
	{
		int i = Random.Range(0, 3);
		switch (i)
		{
			case 0:
				UseSkill2();
				break;
			case 1:
				UseSkill3();
				break;
		}
	}

	public void GetUseYanqingSkillSound()
	{
		int i = Random.Range(0, 2);
		switch (i)
		{
			case 0:
				UseSkillYanqing1();
				break;
			case 1:
				UseSkillYanqing2();
				break;
		}
	}




	void TakeDamageSound1()
	{
		takeDamageAudioSource.clip = takeDamage1;
		takeDamageAudioSource.Play();
	}
	void TakeDamageSound2()
	{
		takeDamageAudioSource.clip = takeDamage2;
		takeDamageAudioSource.Play();
	}
	void TakeDamageSound3()
	{
		takeDamageAudioSource.clip = takeDamage3;
		takeDamageAudioSource.Play();
	}

	void TakeDamageYanqingSound1()
	{
		takeDamageAudioSource.clip = takeDamage4;
		takeDamageAudioSource.Play();
	}
	void TakeDamageYanqingSound2()
	{
		takeDamageAudioSource.clip = takeDamage5;
		takeDamageAudioSource.Play();
	}

	void UseSkill1()
	{
		useSkillAudioSource.clip = useKafkaSkill1;
		useSkillAudioSource.Play();
	}

	void UseSkill2()
	{
		useSkillAudioSource.clip = useServalSkill2;
		useSkillAudioSource.Play();
	}

	void UseSkill3()
	{
		useSkillAudioSource.clip = useServalSkill3;
		useSkillAudioSource.Play();
	}

	void UseSkillYanqing1()
	{
		useSkillAudioSource.clip = useYanqingSkill1;
		useSkillAudioSource.Play();
	}
	void UseSkillYanqing2()
	{
		useSkillAudioSource.clip = useYanqingSkill2;
		useSkillAudioSource.Play();
	}
}
