using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AdsAnimationManager : MonoBehaviour
{
	[Header("References")] private Animator _anim;

	[Header("Weapon Components")] public GameObject M4Prefab;
	public GameObject M4;
	public Transform RightHandIKTarget;
	public Transform LeftHandIKTarget;


	[Header("Multi-Aim Constraints")] public MultiAimConstraint head;
	public MultiAimConstraint chest;
	public MultiAimConstraint spine;

	void Start()
	{
		_anim = GetComponent<Animator>();
	}

	public void SetValues()
	{
		#region M4Transform
		Vector3 m4RotationVector = new Vector3(-50f, 40f, -50f);
		M4.transform.localRotation = Quaternion.Euler(m4RotationVector);
		#endregion

		// #region WeaponIK
		// Vector3 rHandGlobalPos = new Vector3(0.9f, 2.79f, 0.46f);
		//
		// Vector3 rHandRotationVector = new Vector3(0.9f, 2.79f, 0.46f);
		// Vector3 lHandRotationVector = new Vector3(-105.155f, 310.04f, -108.647f);
		//
		// // RightHandIKTarget.position = new Vector3(0.9f, 2.79f, 0.46f);
		// // RightHandIKTarget.rotation = Quaternion.Euler(rHandRotationVector);
		// //
		// // LeftHandIKTarget.position = new Vector3(-0.304f, 3.3f, -0.087f);
		// // LeftHandIKTarget.rotation = Quaternion.Euler(lHandRotationVector);
		//
		// #endregion

		#region Multi-Aim Constraints

		head.data.offset = new Vector3(45, 0, 15);
		chest.data.offset = new Vector3(0, 0, 0);
		spine.data.offset = new Vector3(45, 0, 0);

		#endregion
	}

	public void ResetValues()
	{
		#region M4Transform
		Vector3 m4RotationVector = new Vector3(-90f, 0f, 0f);
		M4.transform.localRotation = Quaternion.Euler(m4RotationVector);
		#endregion

		#region Multi-Aim Constraints

		head.data.offset = new Vector3(-45, 0, 0);
		chest.data.offset = new Vector3(0, 0, 0);
		spine.data.offset = new Vector3(45, -45, 0);

		#endregion
	}
}