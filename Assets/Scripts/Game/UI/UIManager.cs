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

	private ActionStateManager _actionStateManager;

	// [Header("Vignetting")] private Volume postProcessVolume;
	// private Vignette vignette;

	[Header("Bullets")] public TMP_Text ammoIndicator;

	[Header("MouseLock")]
	private bool isCursorLocked = true;

	[Header("Pizzas")] public GameObject pizzaContainer;
	public GameObject pizzaIndicator;
	private PizzaManager _pizzaManager; 
	private List<GameObject> pizzaIndicators = new List<GameObject>();

	[Header("Player Score")] private GameManager _gameManager; 
	public TMP_Text playerScoreText;

	[Header("Player Health")]
	public TMP_Text playerHealthValueText;
	
	private void Awake()
	{
		Instance = this;
	}

	void Start()
		{
			_gameManager = FindObjectOfType<GameManager>();
		_pizzaManager = FindObjectOfType<PizzaManager>();
		StartCoroutine(FindActionStateManagerCoroutine());
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

		playerScoreText.text = _gameManager.playerScore.ToString();
		UpdateHealthEffect();
	}
	
	private IEnumerator FindActionStateManagerCoroutine()
		{
			// Optionally wait for a condition or a delay
			while (_actionStateManager == null)
				{
					_actionStateManager = FindObjectOfType<ActionStateManager>();

					if (_actionStateManager == null)
						{
							Debug.LogWarning("ActionStateManager not found, retrying...");
							yield return new WaitForSeconds(0.5f); // Wait for half a second before retrying
						}
				}

			Debug.Log("ActionStateManager found!");
			// Optionally perform additional setup after finding the manager
		}
	
	void UpdateHealthEffect()
		{
			playerHealthValueText.text = _actionStateManager.playerHealth.ToString();
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