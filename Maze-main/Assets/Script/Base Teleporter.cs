using UnityEngine;

public class BaseTeleporter : MonoBehaviour
{
    public Vector3 teleportDestination = new Vector3(0, 1, 0);
    public Vector3 teleportRotation = new Vector3(0, 0, 0);
    public string playerTag = "Player";
    public Material highlightMaterial;
    public bool showDestinationGizmo = true;

    protected Material originalMaterial;
    protected Renderer cubeRenderer;
    protected bool isPlayerNear = false;

    protected virtual void Start()
    {
        cubeRenderer = GetComponent<Renderer>();

        if (cubeRenderer != null)
        {
            originalMaterial = cubeRenderer.material;
        }

        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("Collider on " + gameObject.name + " should be set as Trigger!");
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNear = true;

            if (highlightMaterial != null && cubeRenderer != null)
            {
                cubeRenderer.material = highlightMaterial;
            }

            Debug.Log("Player entered teleport zone. Teleporting to: " + teleportDestination);
            TeleportPlayer(other.gameObject);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNear = false;

            if (cubeRenderer != null && originalMaterial != null)
            {
                cubeRenderer.material = originalMaterial;
            }
        }
    }

    protected virtual void TeleportPlayer(GameObject player)
    {
        CharacterController charController = player.GetComponent<CharacterController>();

        if (charController != null)
        {
            charController.enabled = false;
            player.transform.position = teleportDestination;
            charController.enabled = true;
        }
        else
        {
            player.transform.position = teleportDestination;
        }

        player.transform.rotation = Quaternion.Euler(0, teleportRotation.y, 0);

        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(teleportRotation.x, 0, 0);
            Debug.Log("Player and camera teleported successfully to " + teleportDestination + " with rotation " +
                      teleportRotation);
        }
        else
        {
            Debug.LogWarning("Camera not found in player children. Only player body was rotated.");
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position, transform.localScale);

        if (showDestinationGizmo)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(teleportDestination, 0.5f);
            Gizmos.DrawLine(transform.position, teleportDestination);

            Gizmos.color = Color.yellow;
            Vector3 forward = Quaternion.Euler(teleportRotation) * Vector3.forward;
            Gizmos.DrawRay(teleportDestination, forward * 2f);
        }
    }
}
