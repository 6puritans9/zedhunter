using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using UnityEngine.Animations.Rigging;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerSetup : MonoBehaviour
{
	public AimStateManager aimStateManager;
	public Cinemachine.CinemachineVirtualCamera VirtualCamera;
	public MovementStateManager movementStateManager;
	public ActionStateManager actionStateManager;
	public PhotonView photonView;
	public RigBuilder rig;

	public WeaponManager weaponManager;
	public WeaponAmmo weaponAmmo;
	public WeaponRecoil weaponRecoil;
	public WeaponBloom weaponBloom;
	public GameObject gun;
	public GameObject sword;

	public Transform TPWeaponHolder;

	public Animator anim;

	public void IsLocalPlayer()
	{
		TPWeaponHolder.gameObject.SetActive(false);

		gun.SetActive(true);
		sword.SetActive(true);

		movementStateManager.enabled = true;
		aimStateManager.enabled = true;
		VirtualCamera.enabled = true;
		actionStateManager.enabled = true;
		weaponManager.enabled = true;
		weaponAmmo.enabled = true;
		weaponRecoil.enabled = true;
		weaponBloom.enabled = true;

		/*anim.enabled = true;
		anim = GetComponent<Animator>();*/
	}

	[PunRPC]
	public void SetTPWeapon(int _weaponIndex)
	{
		if(_weaponIndex == 1)
		{
			TPWeaponHolder.GetChild(0).gameObject.SetActive(false);
			TPWeaponHolder.GetChild(1).gameObject.SetActive(false);
		}
		else if (_weaponIndex == 2)
		{
			TPWeaponHolder.GetChild(0).gameObject.SetActive(true);
			TPWeaponHolder.GetChild(1).gameObject.SetActive(false);
		}
		else if (_weaponIndex == 3)
		{
			TPWeaponHolder.GetChild(0).gameObject.SetActive(false);
			TPWeaponHolder.GetChild(1).gameObject.SetActive(true);
		}
	}
}
