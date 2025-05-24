using UnityEngine;

public class WallImpactDetector : MonoBehaviour
{
    public float impactThreshold = 5f;
    public float impactRadius = 5f;
    public float supportRadius = 1f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > impactThreshold)
        {
            Vector3 impactPoint = collision.contacts[0].point;

            // Find all colliders in the radius
            Collider[] colliders = Physics.OverlapSphere(impactPoint, impactRadius);
            foreach (Collider collider in colliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;

                    // Add force at impact point
                    float distance = Vector3.Distance(rb.position, impactPoint);
                    Vector3 direction = (rb.position - impactPoint).normalized;
                    rb.AddForce(direction * 5f / (distance + 0.1f));
                }
            }
        }
    }

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null && rb.isKinematic)
            {
                // Remove all FixedJoint components
                FixedJoint[] joints = collider.GetComponents<FixedJoint>();
                foreach (FixedJoint joint in joints)
                {
                    Destroy(joint);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}