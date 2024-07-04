public abstract class ActionBaseState
{
	public abstract void EnterState(ActionStateManager actions);
	public abstract void UpdateState(ActionStateManager actions);
	
	public abstract void FixedState(ActionStateManager actions);
}
