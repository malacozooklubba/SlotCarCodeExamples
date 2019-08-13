public interface IVehicleState
{
    void Enter();

    //Called once per frame
    void Execute();

    //Called once per physics update
    void FixedExecute();

    void Exit();
}
