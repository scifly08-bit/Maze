using UnityEngine;

public class BouncyPad : MonoBehaviour
{
    public float BounceForce = 15f;
    public bool useVelocity = false; // Toggle between methods

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COLLISION DETECTED WITH: " + collision.gameObject.name);
        
        // Check if player landed on the bouncy pad
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit bouncy pad!"); // Debug to see if collision is detected

            // Get the player's rigidbody
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Debug.Log("Player velocity before bounce: " + rb.linearVelocity);
                
                if (useVelocity)
                {
                    // Method 1: Direct velocity change (more reliable)
                    Vector3 vel = rb.linearVelocity;
                    vel.y = BounceForce; // Set upward velocity directly
                    rb.linearVelocity = vel;
                }
                else
                {
                    // Method 2: AddForce (original method)
                    // Cancel downward momentum first
                    Vector3 vel = rb.linearVelocity;
                    vel.y = 0;
                    rb.linearVelocity = vel;
                    
                    // Apply bounce force
                    rb.AddForce(Vector3.up * BounceForce, ForceMode.Impulse);
                }

                Debug.Log("Bounce applied! New velocity: " + rb.linearVelocity);
            }
            else
            {
                Debug.LogWarning("No Rigidbody found on player!");
            }
        }
        else
        {
            Debug.Log("Collision object is NOT tagged as Player. Current tag: " + collision.gameObject.tag);
        }
    }
}