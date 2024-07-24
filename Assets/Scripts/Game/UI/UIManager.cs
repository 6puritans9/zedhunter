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

	[Header("MouseLock")]
	private bool isCursorLocked = true;

	[Header("Pizzas")] public GameObject pizzaContainer;
	public GameObject pizzaIndicator;
	private PizzaManager _pizzaManager; 
	private List<GameObject> pizzaIndicators = new List<GameObject>();


	private void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		postProcessVolume = GetComponent<Volume>();
		print($"postProcessVolume: {postProcessVolume}");
		if (postProcessVolume != null && postProcessVolume.profile.TryGet(out vignette))
		{
			vignette.active = true;
			vignette.color.Override(Color.red);
		}

		_pizzaManager = FindObjectOfType<PizzaManager>();
		// Set 5 pizzaIndicators
		for (int i = 0; i < _pizzaManager.totalPizzas; i += 1)
			{
				AddPrefabToLayout();
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
			float vignetteIntensity = Mathf.Lerp(0.5f, 0f, healthPercentage);
			vignette.intensity.Override(vignetteIntensity);
			Debug.Log($"Health: {playerHealth}, Intensity: {vignetteIntensity}");
			print($"postProcessVolume: {postProcessVolume}");
			
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

	public void AddPrefabToLayout()
		{
			GameObject newPizzaIndicator = Instantiate(pizzaIndicator);
			newPizzaIndicator.transform.SetParent(pizzaContainer.transform, false);
			pizzaIndicators.Add(newPizzaIndicator);
		}
        
	public void RemoveLastPizzaIndicator()
		{
			if (pizzaIndicators.Count > 0)
				{
					GameObject lastIndicator = pizzaIndicators[pizzaIndicators.Count - 1];
					pizzaIndicators.RemoveAt(pizzaIndicators.Count - 1);
					Destroy(lastIndicator);
				}
		}
}