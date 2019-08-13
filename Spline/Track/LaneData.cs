using UnityEngine;

public struct LaneData {

    [SerializeField]
    public Vector3[] points;

    public LaneData(Vector3[] points)
    {
        this.points = points;
    }
}
