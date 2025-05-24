using UnityEngine;

// Run this script once on your fractured wall parent
public class SetupFracturedWall : MonoBehaviour 
{
    public float breakForce = 500f;
    
    void Awake() 
    {
        // For each fragment
        foreach(Transform fragment in transform) 
        {
            // Add rigidbody but keep it kinematic until broken
            Rigidbody rb = fragment.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            
            // Add appropriate collider
            fragment.gameObject.AddComponent<MeshCollider>().convex = true;
            
            // Connect to adjacent pieces with fixed joints
            foreach(Transform neighbor in transform) 
            {
                if(neighbor != fragment && AreAdjacent(fragment, neighbor)) 
                {
                    FixedJoint joint = fragment.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = neighbor.GetComponent<Rigidbody>();
                    joint.breakForce = breakForce;
                }
            }
        }
    }

    bool AreAdjacent(Transform a, Transform b)
    {
        return Vector3.Distance(a.position, b.position) < 1.5f;
    }
}
