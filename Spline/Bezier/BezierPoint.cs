using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BezierPoint {

    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;
    public Vector3 p4;

    public BezierPoint(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
        this.p4 = p4;
    }
}
