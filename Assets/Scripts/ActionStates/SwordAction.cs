using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : ActionBaseState
{
	public override void EnterState(ActionStateManager actions)
	{
		/*Debug.Log("Attack1");
		actions.anim.SetTrigger("SwordAction1");
		actions.SwitchState(actions.Default);*/
	}

	public override void FixedState(ActionStateManager actions)
	{
		throw new System.NotImplementedException();
	}

	public override void UpdateState(ActionStateManager actions)
	{
	}
}
