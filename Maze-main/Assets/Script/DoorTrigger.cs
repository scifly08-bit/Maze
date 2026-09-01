using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private DoorOpener leftdoor;
    [SerializeField] private DoorOpener rightdoor;
    [SerializeField] private bool closeOnExit = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            leftdoor.OpenDoor();
            rightdoor.OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (closeOnExit && other.CompareTag("Player"))
        {
            leftdoor.CloseDoor();
            rightdoor.CloseDoor();
        }
    }
}