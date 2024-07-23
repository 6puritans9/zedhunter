using System;
using System.Collections;
using UnityEngine;

public class WallHP : MonoBehaviour
{
	BoxSoundEffect soundEffect;

	public int HP = 100;
	public GameObject Player;

	public bool canAttack = false;

	public static event Action<GameObject> OnWallDestroyed;

	public Material originalMaterial; // 기본 메테리얼
	public Material hitMaterial;      // 피격 시 메테리얼
	public GameObject explosionEffect; // 폭발 이펙트
	public GameObject explosionEffect2; // 폭발 이펙트
	public GameObject landingEffect; // 착지 이펙트

	private Renderer previewRenderer;
	private Collider wallCollider;


	private void Awake()
	{
		soundEffect = GetComponent<BoxSoundEffect>();
	}

	private void OnEnable()
	{
		
		wallCollider = GetComponent<Collider>();

		previewRenderer = GetComponent<Renderer>();
		if (previewRenderer != null && originalMaterial == null)
		{
			originalMaterial = previewRenderer.material;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			// 충돌 지점의 정보를 사용하여 착지 이펙트 위치와 방향 설정
			ContactPoint contact = collision.contacts[0];
			Vector3 position = contact.point;
			Quaternion rotation = Quaternion.LookRotation(contact.normal);

			// 첫 번째 착지 이펙트 생성
			Instantiate(landingEffect, position, rotation);

			// 45도 회전시킨 착지 이펙트 생성
			Quaternion rotation45 = rotation * Quaternion.Euler(0, 45, 0);
			Instantiate(landingEffect, position, rotation45);
		}
	}

	public void TakeDamage(int damage)
	{
		if (HP <= 0) return;

		HP -= damage;
		if (previewRenderer != null)
		{
			Debug.Log("맞음");
			// 벽이 맞을 때 메테리얼을 빨간색으로 변경 후 일정 시간 후 원래 메테리얼로 복구
			previewRenderer.material = hitMaterial;
			StartCoroutine(ResetMaterial());
		}

		if (HP <= 0)
		{
			StartCoroutine(DestroyWallWithEffect()); // 벽이 파괴될 때 효과 적용
		}
	}

	private IEnumerator ResetMaterial()
	{
		yield return new WaitForSeconds(0.5f);
		if (previewRenderer != null)
		{
			previewRenderer.material = originalMaterial; // 원래 메테리얼로 복구
		}
	}

	private IEnumerator DestroyWallWithEffect()
	{
		// 깜빡임 효과 적용
		float blinkDuration = 2.0f; // 깜빡임 효과 지속 시간
		float blinkInterval = 0.1f; // 깜빡임 간격

		for (float t = 0; t < blinkDuration; t += blinkInterval * 2)
		{
			previewRenderer.material = hitMaterial;
			yield return new WaitForSeconds(blinkInterval);
			previewRenderer.material = originalMaterial;
			yield return new WaitForSeconds(blinkInterval);
		}

		// 오브젝트를 보이지 않게 처리
		if (previewRenderer != null) previewRenderer.enabled = false;
		if (wallCollider != null) wallCollider.enabled = false;

		canAttack = true;
		// 폭발 이펙트 생성
		if (explosionEffect != null && explosionEffect2)
		{
			Instantiate(explosionEffect, transform.position, Quaternion.identity);
			Instantiate(explosionEffect2, transform.position, Quaternion.identity);
			soundEffect.explosionAudioSource.PlayOneShot(soundEffect.explosionSound);
		}

		// 폭발 소리가 끝날 때까지 대기
		yield return new WaitWhile(() => soundEffect.explosionAudioSource.isPlaying);

		// 벽 제거
		OnWallDestroyed?.Invoke(this.gameObject);
	}
}
