using System;
using UnityEngine;

public class Driving : IVehicleState
{
    private float currAngle;
    private Vector3 prevPos;
    private Vector3 velocity;

    private float deltaAngle;
    public float DeltaAngle
    {
        get { return deltaAngle; }
        set
        {
            deltaAngle = value;
            GameplayUI.Instance.Angle = deltaAngle;

            if (deltaAngle > vehicleData.angleDeltaToPlayEffect)
            {
                smokeFX.Item1.Emit(1);
                smokeFX.Item2.Emit(1);
            }

            if (deltaAngle > vehicleData.deltaAngleLimit)
            {
                Debug.Log("Delta snap: " + deltaAngle * Time.fixedDeltaTime);
                vehicle.Crash();
            }
        }
    }

    private float currSpeed;
    private float CurrSpeed
    {
        get
        {
            return currSpeed;
        }
        set
        {
            //The actual movement is applied in FixedUpdate since the car i physics driven
            currSpeed = Mathf.Clamp(value, 0, vehicleData.maxSpeed);
            vehicle.Speed = currSpeed;
        }
    }

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

            if (currTargetIndex >= lane.points.Length)
            {
                currTargetIndex = 0;
            }

            vehicle.TargetIndex = currTargetIndex;
        }
    }

    private readonly LaneData lane;
    private readonly Rigidbody hingeRb;
    private readonly HingeJoint hinge;
    private readonly VehicleData vehicleData;
    private readonly Transform hingeTransform;
    private readonly Vehicle vehicle;
    private readonly Tuple<ParticleSystem, ParticleSystem> smokeFX;

    public Driving(
        Vehicle vehicle,
        LaneData lane,
        HingeJoint hinge,
        VehicleData vehicleData,
        int currTargetIndex,
        float currSpeed,
        Tuple<ParticleSystem, ParticleSystem> smokeFX)
    {
        //Components
        this.vehicle = vehicle;
        this.lane = lane;
        this.hinge = hinge;
        this.hingeRb = hinge.GetComponent<Rigidbody>();
        this.hingeTransform = hinge.transform;
        this.vehicleData = vehicleData;
        this.smokeFX = smokeFX;

        //Initial values
        this.currTargetIndex = currTargetIndex;
        CurrSpeed = currSpeed;
    }

    public void Enter()
    {
        Debug.Log("Entered Driving state!");

        currAngle = hinge.connectedBody.transform.eulerAngles.y;
        prevPos = hingeTransform.position;

        GameplayUI.Instance.Setup(vehicleData.deltaAngleLimit);
    }

    //Called once every frame
    public void Execute()
    {
        //If touch or mouse button pressed, accelerate
        if (Input.GetMouseButton(0))
            CurrSpeed += vehicleData.acceleration * Time.deltaTime;
        else
            CurrSpeed -= vehicleData.friction * Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.A))
            vehicle.SwitchLanes(1);
        else if (Input.GetKeyUp(KeyCode.D))
            vehicle.SwitchLanes(-1);
    }

    //Called once every physics update
    public void FixedExecute()
    {
        //Movement
        if (currSpeed > 0)
            Move(currSpeed);

        //Angle
        float newAngle = hinge.connectedBody.transform.eulerAngles.y;
        DeltaAngle = Mathf.Abs(Mathf.DeltaAngle(currAngle, newAngle)) * Time.fixedDeltaTime;
        currAngle = newAngle;
    }

    public void Exit()
    {
        Debug.Log("Exited Driving state!");
    }

    private void Move(float speed)
    {
        Vector3 from = hingeTransform.position;
        float distance = Vector3.Distance(from, lane.points[currTargetIndex]);

        while (speed > distance)
        {
            speed -= distance;
            from = lane.points[currTargetIndex];

            CurrTargetIndex++;

            distance = Vector3.Distance(from, lane.points[currTargetIndex]);
        }

        Vector3 newPos = Vector3.MoveTowards(from, lane.points[currTargetIndex], speed);
        hingeTransform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, -90, 0) * (prevPos - newPos).normalized, Vector3.up);
        prevPos = newPos;
        hingeRb.MovePosition(newPos);
    }
}
