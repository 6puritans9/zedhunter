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

	public Material originalMaterial; // �⺻ ���׸���
	public Material hitMaterial;      // �ǰ� �� ���׸���
	public GameObject explosionEffect; // ���� ����Ʈ
	public GameObject explosionEffect2; // ���� ����Ʈ
	public GameObject landingEffect; // ���� ����Ʈ

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
			// �浹 ������ ������ ����Ͽ� ���� ����Ʈ ��ġ�� ���� ����
			ContactPoint contact = collision.contacts[0];
			Vector3 position = contact.point;
			Quaternion rotation = Quaternion.LookRotation(contact.normal);

			// ù ��° ���� ����Ʈ ����
			Instantiate(landingEffect, position, rotation);

			// 45�� ȸ����Ų ���� ����Ʈ ����
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
			Debug.Log("����");
			// ���� ���� �� ���׸����� ���������� ���� �� ���� �ð� �� ���� ���׸���� ����
			previewRenderer.material = hitMaterial;
			StartCoroutine(ResetMaterial());
		}

		if (HP <= 0)
		{
			StartCoroutine(DestroyWallWithEffect()); // ���� �ı��� �� ȿ�� ����
		}
	}

	private IEnumerator ResetMaterial()
	{
		yield return new WaitForSeconds(0.5f);
		if (previewRenderer != null)
		{
			previewRenderer.material = originalMaterial; // ���� ���׸���� ����
		}
	}

	private IEnumerator DestroyWallWithEffect()
	{
		// ������ ȿ�� ����
		float blinkDuration = 2.0f; // ������ ȿ�� ���� �ð�
		float blinkInterval = 0.1f; // ������ ����

		for (float t = 0; t < blinkDuration; t += blinkInterval * 2)
		{
			previewRenderer.material = hitMaterial;
			yield return new WaitForSeconds(blinkInterval);
			previewRenderer.material = originalMaterial;
			yield return new WaitForSeconds(blinkInterval);
		}

		// ������Ʈ�� ������ �ʰ� ó��
		if (previewRenderer != null) previewRenderer.enabled = false;
		if (wallCollider != null) wallCollider.enabled = false;

		canAttack = true;
		// ���� ����Ʈ ����
		if (explosionEffect != null && explosionEffect2)
		{
			Instantiate(explosionEffect, transform.position, Quaternion.identity);
			Instantiate(explosionEffect2, transform.position, Quaternion.identity);
			soundEffect.explosionAudioSource.PlayOneShot(soundEffect.explosionSound);
		}

		// ���� �Ҹ��� ���� ������ ���
		yield return new WaitWhile(() => soundEffect.explosionAudioSource.isPlaying);

		// �� ����
		OnWallDestroyed?.Invoke(this.gameObject);
	}
}
