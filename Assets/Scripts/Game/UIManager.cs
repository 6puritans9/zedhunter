using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	public Image coolDownImage;
	public TMP_Text coolDownText;

	[Header("Vignetting")] private Volume postProcessVolume;
	private Vignette vignette;

	[Header("Bullets")] public TMP_Text ammoIndicator;

	private bool isCursorLocked = true;



	private void Awake()
	{
		Instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		postProcessVolume = FindObjectOfType<Volume>();
		if (postProcessVolume != null && postProcessVolume.profile.TryGet(out vignette))
		{
			vignette.active = true;
			vignette.color.Override(Color.red);
		}

		SetCursorState(true);
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			// Toggle the cursor state
			isCursorLocked = !isCursorLocked;
			SetCursorState(isCursorLocked);
		}
	}

	public void UpdateHealthEffect(int playerHealth, int maxPlayerHealth)
	{
		if (vignette != null)
		{
			float healthPercentage = (float)playerHealth / maxPlayerHealth;
			float vignetteIntensity = Mathf.Lerp(1.5f, 0f, healthPercentage);
			vignette.intensity.Override(vignetteIntensity);
			/*Debug.Log($"Health: {playerHealth}, Intensity: {vignetteIntensity}");*/
		}
		else
		{
			Debug.LogError("Vignette effect is null!");
		}
	}

	private void SetCursorState(bool isLocked)
	{
		if (isLocked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}


}