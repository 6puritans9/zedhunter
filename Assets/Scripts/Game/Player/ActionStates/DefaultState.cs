// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class DefaultState : ActionBaseState
//     {
//         public override void EnterState(ActionStateManager actions)
//             {
//                 actions.rHandAim.weight = 1;
//                 actions.lHandIK.weight = 1;
//             }
	

//         public override void UpdateState(ActionStateManager actions)
//             {
//                 actions.rHandAim.weight = Mathf.Lerp(actions.rHandAim.weight, 1, 10 * Time.deltaTime);
//                 actions.lHandIK.weight = Mathf.Lerp(actions.lHandIK.weight, 1, 10 * Time.deltaTime);
//                 if (Input.GetKeyDown(KeyCode.R) && CanReload(actions))
//                     {
//                         actions.SwitchState(actions.Reload);
//                     }

//                 if (actions.anim.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.7f
//                     && actions.anim.GetCurrentAnimatorStateInfo(2).IsName("SwordAction1"))
//                     {
//                         actions.anim.SetBool("SwordAction1", false);
//                     }
//                 if (actions.anim.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.7f
//                     && actions.anim.GetCurrentAnimatorStateInfo(2).IsName("SwordAction2"))
//                     {
//                         actions.anim.SetBool("SwordAction2", false);
//                         ActionStateManager.numOfClicks = 0;
//                     }

//                 if (Time.time - actions.lastClickedTime > actions.maxComboDelay)
//                     {
//                         actions.anim.SetBool("SwordAction2", false);
//                         ActionStateManager.numOfClicks = 0;
//                     }
//                 if (Time.time > actions.nextFireTime)
//                     {
//                         actions.anim.SetBool("SwordAction2", false);
//                     }
//                 if (Input.GetMouseButtonDown(0))
//                     {
//                         actions.OnClick();
//                     }
//             }

//         public override void FixedState(ActionStateManager actions)
//             {
//                 throw new System.NotImplementedException();
//             }

//         bool CanReload(ActionStateManager action)
//             {
//                 if (action.ammo.currentAmmo == action.ammo.clipSize) return false;
//                 else if (action.ammo.extraAmmo == 0) return false;
//                 else return true;	
//             }
//     }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultState : ActionBaseState
{
	public override void EnterState(ActionStateManager actions)
	{
		actions.rHandAim.weight = 1;
		actions.lHandIK.weight = 1;
	}
	

	public override void UpdateState(ActionStateManager actions)
	{
		actions.rHandAim.weight = Mathf.Lerp(actions.rHandAim.weight, 1, 10 * Time.deltaTime);
		actions.lHandIK.weight = Mathf.Lerp(actions.lHandIK.weight, 1, 10 * Time.deltaTime);
		if (Input.GetKeyDown(KeyCode.R) && CanReload(actions))
		{
			actions.SwitchState(actions.Reload);
		}

		if (actions.anim.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.7f
			&& actions.anim.GetCurrentAnimatorStateInfo(2).IsName("SwordAction1"))
		{
			actions.anim.SetBool("SwordAction1", false);
		}
		if (actions.anim.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.7f
			&& actions.anim.GetCurrentAnimatorStateInfo(2).IsName("SwordAction2"))
		{
			actions.anim.SetBool("SwordAction2", false);
			ActionStateManager.numOfClicks = 0;
		}

		if (Time.time - actions.lastClickedTime > actions.maxComboDelay)
		{
			actions.anim.SetBool("SwordAction2", false);
			ActionStateManager.numOfClicks = 0;
		}
		if (Time.time > actions.nextFireTime)
		{
			actions.anim.SetBool("SwordAction2", false);
		}
		if (Input.GetMouseButtonDown(0))
		{
			actions.OnClick();
		}
	}

	public override void FixedState(ActionStateManager actions)
	{
		throw new System.NotImplementedException();
	}

	bool CanReload(ActionStateManager action)
	{
		if (action.ammo.currentAmmo == action.ammo.clipSize) return false;
		else if (action.ammo.extraAmmo == 0) return false;
		else return true;	
	}
}
