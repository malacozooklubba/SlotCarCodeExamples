public interface IVehicleState
{
    void Enter();

    //Called one per frame
    void Execute();

    //Called once per physics update
    void FixedExecute();

    void Exit();
}
