using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    public Vector3 teleportDestination = new Vector3(0, 1, 0);
    public Vector3 teleportRotation = new Vector3(0, 0, 0);
    public string playerTag = "Player";
    public Material highlightMaterial;
    public bool showDestinationGizmo = true;
    
    private Material originalMaterial;
    private Renderer cubeRenderer;
    private bool isPlayerNear = false;

    void Start()
    {
        // Get the renderer component
        cubeRenderer = GetComponent<Renderer>();
        
        if (cubeRenderer != null)
        {
            originalMaterial = cubeRenderer.material;
        }
        
        // Verify the cube has a trigger collider
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("Collider on " + gameObject.name + " should be set as Trigger!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is the player
        if (other.CompareTag(playerTag))
        {
            isPlayerNear = true;
            
            // Apply highlight material if available
            if (highlightMaterial != null && cubeRenderer != null)
            {
                cubeRenderer.material = highlightMaterial;
            }
            
            Debug.Log("Player entered teleport zone. Teleporting to: " + teleportDestination);
            
            // Teleport the player
            TeleportPlayer(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Reset material when player leaves
        if (other.CompareTag(playerTag))
        {
            isPlayerNear = false;
            
            if (cubeRenderer != null && originalMaterial != null)
            {
                cubeRenderer.material = originalMaterial;
            }
        }
    }

    void TeleportPlayer(GameObject player)
    {
        // Check if player has a CharacterController
        CharacterController charController = player.GetComponent<CharacterController>();
        
        if (charController != null)
        {
            // Disable CharacterController temporarily to teleport
            charController.enabled = false;
            player.transform.position = teleportDestination;
            charController.enabled = true;
        }
        else
        {
            // If no CharacterController, just move the transform
            player.transform.position = teleportDestination;
        }
        
        // Rotate the player body (Y-axis only for FPS)
        player.transform.rotation = Quaternion.Euler(0, teleportRotation.y, 0);
        
        // Find and rotate the camera (X-axis for looking up/down)
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(teleportRotation.x, 0, 0);
            Debug.Log("Player and camera teleported successfully to " + teleportDestination + " with rotation " + teleportRotation);
        }
        else
        {
            Debug.LogWarning("Camera not found in player children. Only player body was rotated.");
        }
    }

    // Show the trigger area and destination in the Scene view
    void OnDrawGizmos()
    {
        // Draw the trigger zone
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position, transform.localScale);
        
        // Draw the destination point
        if (showDestinationGizmo)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(teleportDestination, 0.5f);
            Gizmos.DrawLine(transform.position, teleportDestination);
            
            // Draw rotation direction arrow
            Gizmos.color = Color.yellow;
            Vector3 forward = Quaternion.Euler(teleportRotation) * Vector3.forward;
            Gizmos.DrawRay(teleportDestination, forward * 2f);
        }
    }
}