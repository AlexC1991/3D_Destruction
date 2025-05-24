using UnityEngine;

public class WallImpactDetector : MonoBehaviour 
{
    public float impactThreshold = 5f;
    
    void OnCollisionEnter(Collision collision) 
    {
        if(collision.relativeVelocity.magnitude > impactThreshold) 
        {
            // Make all fragments non-kinematic
            foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>()) 
            {
                rb.isKinematic = false;
            }
            
            // Add force at impact point
            Vector3 impactPoint = collision.contacts[0].point;
            foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>()) 
            {
                float distance = Vector3.Distance(rb.position, impactPoint);
                Vector3 direction = (rb.position - impactPoint).normalized;
                rb.AddForce(direction * 500f / (distance + 0.1f));
            }
        }
    }
}
