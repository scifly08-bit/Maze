using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private float closedZ = 20f;
    [SerializeField] private float openZ = 22.57f;
    [SerializeField] private float openSpeed = 2f;

    private bool isOpen = false;
    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position;
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;
        targetPosition = new Vector3(transform.position.x, transform.position.y, openZ);
        StopAllCoroutines();
        StartCoroutine(MoveDoor());
    }

    public void CloseDoor()
    {
        if (!isOpen) return;
        isOpen = false;
        targetPosition = new Vector3(transform.position.x, transform.position.y, closedZ);
        StopAllCoroutines();
        StartCoroutine(MoveDoor());
    }

    private System.Collections.IEnumerator MoveDoor()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, openSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }
}