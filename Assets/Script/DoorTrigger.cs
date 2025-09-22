using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    //when trigger is "pressed" it call the opendoor funciton
    public DoorController doorController; // Reference to the DoorController script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorController.OpenDoor(); // Unlock the door when marble enters the trigger
        }
    }
}
