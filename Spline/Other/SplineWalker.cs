using UnityEngine;

public class SplineWalker : MonoBehaviour
{
    public TrackSpline spline;
    private Vector3 velocity;
    private Rigidbody rb;

    public float progress;
    float Progress
    {
        get
        {
            return progress;
        }
        set
        {
            progress = value;

            if (progress > 1f)
            {
                progress = 0;
            }
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public Vector3 Move(float distance)
    {
        //Educated step length guess
        float x = 0.1f;

        Vector3 currentPosition = transform.localPosition;
        float precision = 0.01f;

        float leftPivotT = Progress;
        float rightPivotT = Progress + x;

        float midT = 0;
        Vector3 posAtMidT;

        for (int i = 0; i <= 10; i++)
        {
            midT = (leftPivotT + rightPivotT) * 0.5f;
            posAtMidT = spline.GetPoint(midT);

            float distAtMidT = (posAtMidT - currentPosition).magnitude;

            if (Mathf.Abs(distAtMidT - distance) <= precision)
            {
                break;
            }

            // And here is the trick that ensures we get closer and closer.
            if (distAtMidT < distance)
                leftPivotT = midT;
            else
                rightPivotT = midT;
        }
        Progress = midT;

        Vector3 dir = spline.GetDirection(Progress);
        /**
         * CALCULATE NEW POSITION
         * */
        Vector3 newPosition = spline.GetPoint(Progress);
        rb.MovePosition(newPosition);
        rb.MoveRotation(Quaternion.LookRotation(dir, transform.up));

        return newPosition - currentPosition;
    }
}