using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    public int clipSize;
    public int extraAmmo;
    public int currentAmmo;

    public AudioClip magInSound;
    public AudioClip magOutSound;
    public AudioClip releaseSlideSound;

    [Header("Canvas")] public TMP_Text ammoIndicator;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = clipSize;
    }

	private void Update()
	{
        if (Input.GetKeyDown(KeyCode.R)) Reload();
        UpdateAmmoIndicator();
    }

	public void Reload()
    {
        if (extraAmmo >= clipSize)
        {
            int ammoToReload = clipSize - currentAmmo;
            extraAmmo -= ammoToReload;
            currentAmmo += ammoToReload;
        }
        else if(extraAmmo > 0)
        {
            if(extraAmmo + currentAmmo > clipSize)
            {
                int leftOverAmmo = extraAmmo + currentAmmo - clipSize;
                extraAmmo = leftOverAmmo;
                currentAmmo = clipSize;
            }
            else
            {
                currentAmmo += extraAmmo;
                extraAmmo = 0;
            }
        }
    }

    private void UpdateAmmoIndicator()
        {
            if (UIManager.Instance.ammoIndicator)
                UIManager.Instance.ammoIndicator.text = $"{currentAmmo.ToString()} / {clipSize.ToString()}";
        }
}
