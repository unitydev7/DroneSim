using UnityEngine;

public class DroneYawTrail : MonoBehaviour
{
    public Transform trailAnchor;        // assign the empty child
    public TrailRenderer trail;          // assign the trail renderer
    public float radius = 2f;            // circle radius

    private float accumulatedYaw = 0f;
    private float lastYaw;

    void Start()
    {
        if (trail != null) trail.emitting = false;
        lastYaw = transform.eulerAngles.y;
    }

    void Update()
    {
        float currentYaw = transform.eulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(lastYaw, currentYaw);
        lastYaw = currentYaw;

        // Update accumulated yaw
        accumulatedYaw += deltaYaw;

        // Check if yawing
        if (Mathf.Abs(deltaYaw) > 0.01f && Mathf.Abs(accumulatedYaw) < 360f)
        {
            if (trail != null) trail.emitting = true;

            // Move anchor in circle path
            float angleRad = Mathf.Deg2Rad * accumulatedYaw;
            Vector3 offset = new Vector3(Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad)) * radius;
            trailAnchor.localPosition = offset;
        }
        else if (Mathf.Abs(accumulatedYaw) >= 360f)
        {
            // Completed circle -> stop and reset
            if (trail != null) trail.emitting = false;
            accumulatedYaw = 0f;
            trail.Clear();
            trailAnchor.localPosition = Vector3.zero;
        }
        else
        {
            if (trail != null) trail.emitting = false;
        }
    }
}
