using System;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public bool GODMODE = false;

    public VehicleData data;

    private CarComponents comps;
    private Vector3 velocity;
    private VechicleStateMachine stateMachine = new VechicleStateMachine();
    private LaneData[] lanes;

    public int CurrLaneIndex { get; set; }
    public int TargetIndex { get; set; }
    public float Speed { get; set; }

    // Use this for initialization
    public void Init(CarComponents carComponents, LaneData[] lanes, int laneIndex)
    {
        comps = carComponents;
        CurrLaneIndex = laneIndex;
        this.lanes = lanes;

        stateMachine.ChangeState(
            new Driving(
                this,
                lanes[CurrLaneIndex],
                comps.hinge,
                data,
                TargetIndex,
                Speed,
                comps.smokeEffects));
    }

    private void Update()
    {
        stateMachine.ExecuteStateUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.ExecuteStateFixedUpdate();
    }

    public void SwitchLanes(int increment)
    {
        int newLaneIndex = Mathf.Clamp(CurrLaneIndex + increment, 0, lanes.Length - 1);

        if (newLaneIndex != CurrLaneIndex)
        {
            stateMachine.ChangeState(
                new SwitchingLanes(
                    this,
                    lanes[CurrLaneIndex],
                    lanes[newLaneIndex],
                    data,
                    comps.hinge,
                    comps.carBody,
                    TargetIndex,
                    Speed));

            CurrLaneIndex = newLaneIndex;
        }
    }

    public void Crash()
    {
        stateMachine.ChangeState(
            new Crashed(
                comps.hinge.connectedBody,
                comps.hinge,
                Vector3.zero));

        GameplayUI.Instance.Crash();
    }

    public void LaneSwitchDone()
    {
        stateMachine.ChangeState(
            new Driving(
                this,
                lanes[CurrLaneIndex],
                comps.hinge,
                data,
                TargetIndex,
                Speed,
                comps.smokeEffects));
    }
}
