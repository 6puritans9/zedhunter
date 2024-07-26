using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCameraPos : MonoBehaviour
{
	public static ShakeCameraPos Instance;
	public Transform camTransform;
	public float shakeDuration = 0.5f;
	public float shakeAmount = 0.5f;
	public float decreaseFactor = 1.0f;

	Vector3 originalPos;

	void Awake()
	{
		Instance = this;
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}

	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	public void TriggerShake()
	{
		StartCoroutine(Shake());
	}

	IEnumerator Shake()
	{
		float elapsed = 0.0f;

		while (elapsed < shakeDuration)
		{
			float x = Random.Range(originalPos.x - 0.6f, originalPos.x + 0.2f) * shakeAmount;
			float y = Random.Range(originalPos.y + 1, originalPos.y + 2) * shakeAmount;

			camTransform.localPosition = new Vector3(x, y, originalPos.z);

			elapsed += Time.deltaTime;

			yield return null;
		}

		camTransform.localPosition = originalPos;
	}
}
