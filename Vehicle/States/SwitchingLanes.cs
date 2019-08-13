using UnityEngine;

public class SwitchingLanes : IVehicleState
{
    private float progress = 0;
    private float timer = 0;
    private Vector3 prevPos;
    private float maxAngleDelta;

    private int currTargetIndex;
    private int CurrTargetIndex
    {
        get
        {
            return currTargetIndex;
        }
        set
        {
            currTargetIndex = value;

            if (currTargetIndex >= lane2.points.Length)
            {
                currTargetIndex = 0;
            }

            vehicle.TargetIndex = currTargetIndex;
        }
    }

    private readonly Vehicle vehicle;
    private readonly LaneData lane1;
    private readonly LaneData lane2;
    private readonly VehicleData vehicleData;
    private readonly HingeJoint hinge;
    private readonly Transform hingeTransform;
    private readonly Rigidbody hingeRb;
    private readonly Rigidbody carBody;
    private readonly float currSpeed;

    public SwitchingLanes(Vehicle vehicle, LaneData lane1, LaneData lane2, VehicleData vehicleData, HingeJoint hinge, Rigidbody carBody, int currTargetIndex, float currSpeed)
    {
        this.vehicle = vehicle;
        this.lane1 = lane1;
        this.lane2 = lane2;
        this.vehicleData = vehicleData;
        this.hinge = hinge;
        this.hingeRb = hinge.GetComponent<Rigidbody>();
        this.carBody = carBody;
        this.hingeTransform = hinge.transform;
        this.currTargetIndex = currTargetIndex;
        this.currSpeed = currSpeed;
    }

    public void Enter()
    {
        Debug.Log("Switching lanes!");
    }

    public void Execute()
    {
        timer += Time.deltaTime;
        progress = timer / vehicleData.laneSwitchSpeed;
    }

    public void FixedExecute()
    {
        if (currSpeed > 0)
            Move(currSpeed);

        if (progress >= 1)
            vehicle.LaneSwitchDone();
    }

    public void Exit()
    {
        Debug.Log("Done switching lanes!");
    }

    private void Move(float speed)
    {
        Vector3 from = hingeTransform.position;
        Vector3 to = Vector3.Lerp(lane1.points[CurrTargetIndex], lane2.points[CurrTargetIndex], progress);
        float distance = Vector3.Distance(from, to);

        while (speed > distance)
        {
            speed -= distance;
            from = Vector3.Lerp(lane1.points[CurrTargetIndex], lane2.points[CurrTargetIndex], progress);

            CurrTargetIndex++;

            to = Vector3.Lerp(lane1.points[CurrTargetIndex], lane2.points[CurrTargetIndex], progress);
            distance = Vector3.Distance(from, to);
        }

        Vector3 newPos = Vector3.MoveTowards(from, to, speed);
        Quaternion hingeAngle = Quaternion.LookRotation(Quaternion.Euler(0, -90, 0) * (prevPos - newPos).normalized, Vector3.up);
        prevPos = newPos;

        hingeRb.MoveRotation(hingeAngle);
        hingeRb.MovePosition(newPos);
        carBody.MovePosition(newPos);
    }
}
