using UnityEngine;

public class Crashed : IVehicleState
{
    const float timeUntilReset = 1f;
    private float crashedDuration = 0f;

    private readonly Rigidbody rb;
    private readonly HingeJoint hinge;

    public Crashed(Rigidbody rb, HingeJoint hinge, Vector3 crashVelocity)
    {
        this.rb = rb;
        this.hinge = hinge;
    }

    public void Enter()
    {
        Debug.Log("CRASHED!");

        hinge.breakForce = 0;
        rb.drag = 0;
        rb.angularDrag = 0;
    }

    public void Execute()
    {
        if (crashedDuration > timeUntilReset ||
            Input.GetKeyUp(KeyCode.R) ||
            Input.touchCount > 2)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        crashedDuration += Time.deltaTime;
    }

    public void FixedExecute() { }

    public void Exit() { }
}
