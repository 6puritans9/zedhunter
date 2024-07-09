using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using UnityEngine.Animations.Rigging;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerSetup : MonoBehaviour
{
	
	public Cinemachine.CinemachineVirtualCamera VirtualCamera;
	public Cinemachine3rdPersonAim cinemachine3RdPersonAim;

	public AimStateManager aimStateManager;
	public MovementStateManager movementStateManager;
	public ActionStateManager actionStateManager;
	public WallBuilder wallBuilder;

	public WeaponManager weaponManager;
	public WeaponAmmo weaponAmmo;
	public WeaponRecoil weaponRecoil;
	public WeaponBloom weaponBloom;
	public GameObject gun;

	public Transform TPWeaponHolder;

	public Rig gunRig;

	public Animator anim;

	public void IsLocalPlayer()
	{
		//TPWeaponHolder.gameObject.SetActive(false);

		gun.SetActive(true);

		movementStateManager.enabled = true;
		aimStateManager.enabled = true;
		actionStateManager.enabled = true;
		wallBuilder.enabled = true;
		
		VirtualCamera.enabled = true;
		cinemachine3RdPersonAim.enabled = true;


		gunRig.weight = 0;

		weaponManager.enabled = true;
		weaponAmmo.enabled = true;
		weaponRecoil.enabled = true;
		weaponBloom.enabled = true;

	}

	[PunRPC]
	public void SetTPWeapon(int _weaponIndex)
	{
		if(_weaponIndex == 1)
		{
			TPWeaponHolder.GetChild(0).gameObject.SetActive(false);
			TPWeaponHolder.GetChild(1).gameObject.SetActive(false);
			gunRig.weight = 0;
		}
		else if (_weaponIndex == 2)
		{
			TPWeaponHolder.GetChild(0).gameObject.SetActive(true);
			TPWeaponHolder.GetChild(1).gameObject.SetActive(false);
			gunRig.weight = 1;
		}
	}
}
