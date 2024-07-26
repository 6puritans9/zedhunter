using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCrosshairSprite : MonoBehaviour
{
	public static ChangeCrosshairSprite Instance;
	public Image crosshair;
	public Image crosshair2;
	public Sprite crosshairSprite;
	public Sprite crosshairSprite2;

	Color colorOrigin;
	Color alphaZeroColor;
	private void Awake()
	{
		Instance = this;
		crosshair = GetComponent<Image>();

		crosshair.sprite = crosshairSprite;

		colorOrigin = new Color(crosshair2.color.r, crosshair2.color.g, crosshair2.color.b, 1);
		alphaZeroColor = new Color(crosshair2.color.r, crosshair2.color.g, crosshair2.color.b, 0);

		crosshair2.color = alphaZeroColor;
	}

	public void ChangeSprite(int idx)
	{
		switch (idx)
		{
			case 0:
				break;
			case 1:
				crosshair2.color = colorOrigin;
				break;
		}

		StartCoroutine(Reset());
	}

	IEnumerator Reset()
	{
		yield return new WaitForSeconds(0.2f);
		crosshair2.color = alphaZeroColor;
	}
}
