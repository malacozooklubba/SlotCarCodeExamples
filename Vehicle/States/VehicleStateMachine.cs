public class VechicleStateMachine
{
    private IVehicleState currentState;
    private IVehicleState previousState;

    public void ChangeState(IVehicleState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
            previousState = currentState;
        }

        currentState = newState;
        currentState.Enter();
    }

    public void ExecuteStateUpdate()
    {
        if (currentState != null)
            currentState.Execute();
    }

    public void ExecuteStateFixedUpdate()
    {
        if (currentState != null)
            currentState.FixedExecute();
    }

    public void ReturnToPrevious()
    {
        ChangeState(previousState);
    }
}
